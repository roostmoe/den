using Aspire.Hosting.JavaScript;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Aspire.Hosting;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class Extensions {
    public static IResourceBuilder<TResource> WithBun<TResource>(this IResourceBuilder<TResource> resource, bool install = true, string[]? installArgs = null)
        where TResource : JavaScriptAppResource
    {
        ArgumentNullException.ThrowIfNull(resource, nameof(resource));
        string workingDirectory = resource.Resource.WorkingDirectory;
        bool flag = File.Exists(Path.Combine(workingDirectory, "bun.lock"));
        installArgs ??= GetDefaultBunInstallArgs(resource, flag);

        string text = "package.json";
        if (flag)
        {
            text += " bun.lock";
        }

        IResourceBuilder<TResource> resourceBuilder = resource.WithAnnotation(new JavaScriptPackageManagerAnnotation("bun", "run")
        {
            PackageFilesPatterns =
            {
                new CopyFilePattern(text, "./")
            },
            CommandSeparator = null
        });

        string text2 = "install";
        string[] array = installArgs;
        int num = 0;
        string[] array2 = new string[1 + array.Length];
        array2[num] = text2;
        num++;
        ReadOnlySpan<string> readOnlySpan = new(array);
        readOnlySpan.CopyTo(new Span<string>(array2).Slice(num, readOnlySpan.Length));
        num += readOnlySpan.Length;
        resourceBuilder.WithAnnotation(new JavaScriptInstallCommandAnnotation(array2));
        AddInstaller(resource, install);
        return resource;
    }

    private static string[] GetDefaultBunInstallArgs(IResourceBuilder<JavaScriptAppResource> resource, bool hasBunLock) =>
        resource.ApplicationBuilder.ExecutionContext.IsPublishMode && hasBunLock
            ? ["--frozen-lockfile"]
            : [];

    private static void AddInstaller<TResource>(IResourceBuilder<TResource> resource, bool install) where TResource : JavaScriptAppResource
    {
        // Only install packages if in run mode
        if (resource.ApplicationBuilder.ExecutionContext.IsRunMode)
        {
            // Check if the installer resource already exists
            var installerName = $"{resource.Resource.Name}-installer";
            resource.ApplicationBuilder.TryCreateResourceBuilder<JavaScriptInstallerResource>(installerName, out var existingResource);

            if (!install)
            {
                if (existingResource != null)
                {
                    // Remove existing installer resource if install is false
                    resource.ApplicationBuilder.Resources.Remove(existingResource.Resource);
                    resource.Resource.Annotations.OfType<WaitAnnotation>()
                        .Where(w => w.Resource == existingResource.Resource)
                        .ToList()
                        .ForEach(w => resource.Resource.Annotations.Remove(w));
                    resource.Resource.Annotations.OfType<JavaScriptPackageInstallerAnnotation>()
                        .ToList()
                        .ForEach(a => resource.Resource.Annotations.Remove(a));
                }
                else
                {
                    // No installer needed
                }
                return;
            }

            if (existingResource is not null)
            {
                // Installer already exists
                return;
            }

            var installer = new JavaScriptInstallerResource(installerName, resource.Resource.WorkingDirectory);
            var installerBuilder = resource.ApplicationBuilder.AddResource(installer)
                .WithParentRelationship(resource.Resource)
                .ExcludeFromManifest();

            resource.ApplicationBuilder.Eventing.Subscribe<BeforeStartEvent>((_, _) =>
            {
                // set the installer's working directory to match the resource's working directory
                // and set the install command and args based on the resource's annotations
                if (!resource.Resource.TryGetLastAnnotation<JavaScriptPackageManagerAnnotation>(out var packageManager) ||
                    !resource.Resource.TryGetLastAnnotation<JavaScriptInstallCommandAnnotation>(out var installCommand))
                {
                    throw new InvalidOperationException("JavaScriptPackageManagerAnnotation and JavaScriptInstallCommandAnnotation are required when installing packages.");
                }

                installerBuilder
                    .WithCommand(packageManager.ExecutableName)
                    .WithWorkingDirectory(resource.Resource.WorkingDirectory)
                    .WithArgs(installCommand.Args);

                return Task.CompletedTask;
            });

            // Make the parent resource wait for the installer to complete
            resource.WaitForCompletion(installerBuilder);

            resource.WithAnnotation(new JavaScriptPackageInstallerAnnotation(installer));
        }
    }
}