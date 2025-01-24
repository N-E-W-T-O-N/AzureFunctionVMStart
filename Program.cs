using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Network;
using Azure.ResourceManager.Resources;


Console.WriteLine($"C# Timer trigger function executed at: {DateTime.Now}");

// Specify your Azure subscription ID, resource group, VM name
var subscriptionId = "SubScriptionID";
var resourceGroupName = "RG-NAME";
var vmName = "VM-Name";

//string resourceId = $"/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Compute/virtualMachines/{vmName}";

//APPROCH 1  , Provide all information thus don't have all info
ArmClient armClient = new ArmClient(new DefaultAzureCredential());


ResourceIdentifier virtualMachineResourceId = VirtualMachineResource.CreateResourceIdentifier(subscriptionId, resourceGroupName, vmName);

VirtualMachineResource virtualMachine = armClient.GetVirtualMachineResource(virtualMachineResourceId);
WaitUntil w = WaitUntil.Completed;
//await virtualMachine.RestartAsync(WaitUntil.Completed);

Console.WriteLine(virtualMachine.HasData);
await virtualMachine.PowerOnAsync(WaitUntil.Completed);
//Console.WriteLine(virtualMachine.Data.ProvisioningState);


//APPROCH 2 through SubscrptionID & Resource
ResourceIdentifier resourceGroupResourceId = ResourceGroupResource.CreateResourceIdentifier(subscriptionId, resourceGroupName);
Console.WriteLine(resourceGroupResourceId.ToString());

ResourceGroupResource resourceGroupResource = armClient.GetResourceGroupResource(resourceGroupResourceId);

// get the collection of this VirtualMachineResource

var Vm = (await resourceGroupResource.GetVirtualMachineAsync(vmName)).Value;
await Vm.PowerOnAsync(WaitUntil.Completed);

//APPROCH 3 iteration
resourceGroupResourceId = ResourceGroupResource.CreateResourceIdentifier(subscriptionId, resourceGroupName);
VirtualMachineCollection collection = resourceGroupResource.GetVirtualMachines();

// With ListAsync(), we can get a list of the virtual machines
AsyncPageable<VirtualMachineResource> response = collection.GetAllAsync();
await foreach (VirtualMachineResource vm in response)
{
    Console.WriteLine(vm.Data.Name);
    Console.WriteLine(vm.HasData);
    Console.WriteLine(vm.Data.Location);
    Console.WriteLine(vm.Data.Id);
    Console.WriteLine(vm.Data.VmId);

}
 
