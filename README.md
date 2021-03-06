# Xperience Font Awesome Integrator

This project is a .NET global tool that helps integrate [Font Awesome 5](https://fontawesome.com/download) into the [Kentico Xperience](https://xperience.io/) content management application.

It requires the [.NET Core SDK](https://dotnet.microsoft.com/download) to run at the command line (which should already be installed with Visual Studio).

[View the NuGet Package](https://www.nuget.org/packages/WiredViews.Xperience.FontAwesomeIntegrator/)

## Adding Icons to Xperience

Kentico Xperience already provides [a set of icons](https://devnet.kentico.com/docs/icon-list/index.html) for [developers and content editors to use](https://docs.kentico.com/developing-websites/defining-website-content-structure/managing-page-types/changing-page-type-icons) in the content management application.

Icons can also be [added to the default set](https://docs.kentico.com/k12sp/custom-development/working-with-font-icons), but this requires some customization if you are trying to integrate the Font Awesome 5 icons.

This global tool will handle all the file/folder processing for you. It works with the Free and Pro versions of Font Awesome 5.

## Examples

First install the global tool:

```bash
dotnet tool install --global WiredViews.Xperience.FontAwesomeIntegrator
```

Extract the .zip file [downloaded from Font Awesome's site](https://fontawesome.com/download).

Execute the tool to process and copy files to your CMS application

> `fpath` is the path to the extracted Font Awesome content folder (containing `/css`, `/webfonts`, and `/metadata` folders).
> `cpath` is the path to your Xperience CMS application folder

```bash
> xperience-fa-integrator -fpath C:\dev\fontawesome-free-5.14.0-web -cpath C:\dev\Xperience\CMS
```

This tool does not include the copied files into your project. You will need to either edit the `CMSApp.csproj` project file and add the entries or use the [Visual Studio Solution Explorer to add the files](http://f5debug.net/how-to-add-existing-folder-or-directory-to-visual-studio-project-solution/).
If the files are not added to the project, they will not be included during deployments.

## References

### Font Awesome

- [Icon Library](https://fontawesome.com/icons?d=gallery)
- [Free Icons Download](https://fontawesome.com/download)

### .NET Global Tool Docs

- [.NET Core SDK](https://dotnet.microsoft.com/download)
- [How to use a Global Tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools-how-to-use)

### Kentico Docs

- [Xperience Icon List](https://devnet.kentico.com/docs/icon-list/index.html)
- [Changing Page Type Icons](https://docs.kentico.com/developing-websites/defining-website-content-structure/managing-page-types/changing-page-type-icons)
- [Customizing Font Icons in Xperience](https://docs.kentico.com/k12sp/custom-development/working-with-font-icons)

### Blog Posts

- [Kentico CMS Quick Tip: Using Font Awesome Icons for Custom Page Types ](https://dev.to/wiredviews/kentico-cms-quick-tip-using-font-awesome-icons-for-custom-page-types-3fcb) to see the details of about the creation of this project.