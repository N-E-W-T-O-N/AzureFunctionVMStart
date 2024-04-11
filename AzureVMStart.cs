using System;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Compute;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace VM_Start
{
    public class AzureVMStart
    {
        string  subscriptionId {get;set;}
        string  resourceGroupName {get;set;}
        string vmName {get;set;}
        public AzureVMStart()
        {
            subscriptionId = Environment.GetEnvironmentVariable("SUBSCRIPTION_ID").ToString();
            resourceGroupName = Environment.GetEnvironmentVariable("RESOURCE_GROUP_NAME").ToString();
            vmName = Environment.GetEnvironmentVariable("VM_NAME").ToString();
        }
        

        //string resourceId = $"/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Compute/virtualMachines/{vmName}";
        [FunctionName("AzureVMStart")]
        public async Task Run([TimerTrigger("0 0 4 * * 1-5")]TimerInfo myTimer, ILogger log)
        {
            try{
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            ArmClient armClient = new ArmClient(new DefaultAzureCredential());

            ResourceIdentifier virtualMachineResourceId = VirtualMachineResource.CreateResourceIdentifier(subscriptionId, resourceGroupName, vmName);

            VirtualMachineResource virtualMachine = armClient.GetVirtualMachineResource(virtualMachineResourceId);
            
            await virtualMachine.PowerOnAsync(WaitUntil.Completed);
            //await virtualMachine.RestartAsync(WaitUntil.Completed);
            log.LogInformation($" VM {vmName} is Running Now ");
            }
            catch(Exception ex )
            {
                log.LogError(ex.Message);
            }
        }
    }
}
