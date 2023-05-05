# A Reimplementation of the Remote Code Executor server in .NET 👋

> This is a reimplementation of the remote code executor project I'd built a couple years ago. The original project was in Node, but has been reimplemented using .NET this time around. 
>There were a couple of issues with the original project, like using linux commands for adding and removing files, not to mention saving the files locally first. I've used the Docker SDK for .NET this time for a more generalized experience, and I've done away with saving the files locally.
>The input and code files are written directly to the Docker container, thus having no local links.
> The server itself is basically the same as before, expect 2 responses, one with the stdout and the other with the stderr.
> The folder structure follows the pattern of Controllers, Repositories/Services and Models.

## Author

👤 **Rajat Maheshwari**

* Website: https://rajatmaheshwari.tech/
* Github: [@rajatmaheshwari2512](https://github.com/rajatmaheshwari2512)
* LinkedIn: [@https:\/\/linkedin.com\/in\/rajatmaheshwari2512](https://linkedin.com/in/https:\/\/linkedin.com\/in\/rajatmaheshwari2512)

## Show your support

Give a ⭐️ if this project helped you!


***