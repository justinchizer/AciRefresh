using System;
using Microsoft.Azure.Management.ContainerInstance.Fluent;
using Microsoft.Azure.Management.ContainerInstance.Fluent.Models;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System.Threading.Tasks;


namespace refreshtest
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var credentials = SdkContext.AzureCredentialsFactory
            .FromServicePrincipal("", //Azure Id
            "",// Azure secret
            "",// Azure Tenant
            AzureEnvironment.AzureGlobalCloud);

            Console.WriteLine("credentials done");

            var azure = Azure
            .Configure()
            .Authenticate(credentials)
            .WithDefaultSubscription();

            string containerGroupName = ""; //name of the container group

            Console.WriteLine("azure config");
            Console.WriteLine("start ACI");

            IContainerGroup containerGroup = azure.ContainerGroups.Define(containerGroupName)
                                    .WithRegion(Region.USWest2)
                                    .WithExistingResourceGroup("newdotnet")
                                    .WithLinux()
                                    .WithPrivateImageRegistry("newdotcr.azurecr.io", "newdotcr", "") //password sent in teams
                                    .WithoutVolume()
                                    .DefineContainerInstance(containerGroupName)
                                        .WithImage("newdotcr.azurecr.io/newdotcr")
                                        .WithExternalTcpPort(80)
                                        .WithCpuCoreCount(1)
                                        .WithMemorySizeInGB(2.5)
                                        .WithEnvironmentVariable("SOURCECONTROL", "GitHub")
                                        .WithEnvironmentVariable("TEMPLATE_NAME", "Classlib")
                                        .WithEnvironmentVariable("GITHUB_NAME", "")//user name
                                        .WithEnvironmentVariable("VSTS_NAME", " ")
                                        .WithEnvironmentVariable("REPO", "")// repo name
                                        .WithEnvironmentVariable("EMAIL", "")//email
                                        .WithEnvironmentVariable("TOKENENGINE", "")// set breakpoint in startup to grab token
                                        .WithEnvironmentVariable("DESCRIPTION", " ")
                                        .WithEnvironmentVariable("USE_TRAVIS", "false")
                                        .WithEnvironmentVariable("ENCRYPTION_ENABLED", "false")
                                        .WithEnvironmentVariable("USE_KEYVAULT", "false")
                                        .WithEnvironmentVariable("KEYVAULT_NAME", " ")
                                        .WithEnvironmentVariable("AD_ID", " ")
                                        .WithEnvironmentVariable("AD_SECRET", " ")
                                        .WithEnvironmentVariable("AES_KEY", " ")
                                        .WithEnvironmentVariable("AES_IV", " ")
                                        .WithEnvironmentVariable("HMAC_KEY", " ")
                                        .WithEnvironmentVariable("BASEURL", " ")// set up ngrok
                                        .WithEnvironmentVariable("PROJECTID", "1")
                                        .Attach()
                                    .WithRestartPolicy(ContainerGroupRestartPolicy.Never)
                                    .Create();

            Console.WriteLine("ACI Created");
            //Console.WriteLine(containerGroup.ToString());
            Console.WriteLine($" Exit before refresh: {containerGroup.Inner.Containers[0].InstanceView.CurrentState.ExitCode.ToString()}");


            int i = 0;

            while (containerGroup.Inner.Containers[0].InstanceView.CurrentState.ExitCode != 0)
            {
                Console.WriteLine($" Exit: {containerGroup.Inner.Containers[0].InstanceView.CurrentState.ExitCode.ToString()}");
                containerGroup.Refresh();
                Console.WriteLine(containerGroup.Inner.Containers[0].InstanceView.CurrentState.DetailStatus);
                System.Threading.Thread.Sleep(3000);
                i++;
            }
            Console.WriteLine(containerGroup.GetLogContent("refresh4"));
            Console.WriteLine(containerGroup.Inner.Containers[0].InstanceView.CurrentState.DetailStatus);
            Console.WriteLine(i);
            Console.WriteLine("done");
            while(true)
            {

            }


        }
    }
}
