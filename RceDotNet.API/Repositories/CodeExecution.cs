using Docker.DotNet;
using Docker.DotNet.Models;

namespace RceDotNet.API.Repositories
{
    public class CodeExecution : ICodeExecution
    {
        private readonly DockerClient dockerClient;
        private readonly Dictionary<string, string> languageToDocker = new Dictionary<string, string>();
        private readonly Dictionary<string, string> languageToCommand = new Dictionary<string, string>();
        public CodeExecution()
        {
            //Setup docker client
            dockerClient = new DockerClientConfiguration().CreateClient();
            //Setup language to docker image mapping
            languageToDocker.Add("cpp", "cpp");
            languageToDocker.Add("java", "java");
            languageToDocker.Add("py", "python");
            //Setup language to command execution mapping
            languageToCommand.Add("cpp","g++ -fdiagnostics-color=never test.cpp && a.out<input.txt");
            languageToCommand.Add("java", "javac test.java && java test");
            languageToCommand.Add("py", "python3 test.py<input.txt");
        }
        public async Task<List<string>> ExecuteCodeAsync(string code, string language,string? input = "")
        {
            try
            {
                //Create container
                var container = await dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters()
                {
                    Image = $"{languageToDocker[language]}:v1",
                    Cmd = new List<string>() { "/bin/bash" },
                    AttachStdin = true,
                    AttachStderr = true,
                    AttachStdout = true,
                    Tty = true,
                });
                //Start container, and watch if it started successfully
                bool containerStarted = await dockerClient.Containers.StartContainerAsync(container.ID, new ContainerStartParameters());
                if (containerStarted)
                {
                    //Write the code file to directory inside the container
                    var execToWriteFile = dockerClient.Exec.ExecCreateContainerAsync(container.ID, new ContainerExecCreateParameters()
                    {
                        AttachStderr = true,
                        AttachStdout = true,
                        AttachStdin = true,
                        WorkingDir="/usr/src/app",
                        Cmd = new List<string>()
                        {
                            "bash","-c",$"echo -e \"{code}\">test.{language}"
                        }
                    });
                    await dockerClient.Exec.StartAndAttachContainerExecAsync(execToWriteFile.Result.ID, tty: true);
                    //Write input file to directory inside the container
                    var execToWriteInputFile = dockerClient.Exec.ExecCreateContainerAsync(container.ID, new ContainerExecCreateParameters()
                    {
                        AttachStderr = true,
                        AttachStdout = true,
                        AttachStdin = true,
                        WorkingDir = "/usr/src/app",
                        Cmd = new List<string>()
                        {
                            "bash","-c",$"echo -e \"{input}\">input.txt"
                        },
                    });
                    await dockerClient.Exec.StartAndAttachContainerExecAsync(execToWriteInputFile.Result.ID, tty: true);
                    //Run the code
                    var runCodeExec = dockerClient.Exec.ExecCreateContainerAsync(container.ID, new ContainerExecCreateParameters()
                    {
                        AttachStderr = true,
                        AttachStdout = true,
                        AttachStdin = true,
                        WorkingDir = "/usr/src/app",
                        Cmd = new List<string>()
                        {
                            "bash","-c",$"{languageToCommand[language]}"
                        },
                        Tty = true,
                    });
                    //Read the output form the stream
                    var outputStream = await dockerClient.Exec.StartAndAttachContainerExecAsync(runCodeExec.Result.ID, false);
                    var output = await outputStream.ReadOutputToEndAsync(CancellationToken.None);
                    //Don't need to await this because removal of container is not important from a user's perspective
                    dockerClient.Containers.RemoveContainerAsync(container.ID, new ContainerRemoveParameters()
                    {
                        Force = true
                    });
                    //Return the output
                    return new List<string>() { output.stdout, output.stderr };
                }
                else
                {
                    //Container failed to start, inform user
                    return new List<string>() { "", "Container failed to start" };
                }
            }
            catch (Exception e)
            {
                //General catchall for exceptions
                return new List<string>() { "", e.Message };
            }
        }
    }
}
