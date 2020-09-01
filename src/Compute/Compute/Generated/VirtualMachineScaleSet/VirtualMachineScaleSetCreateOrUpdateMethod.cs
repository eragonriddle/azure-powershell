//
// Copyright (c) Microsoft and contributors.  All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//
// See the License for the specific language governing permissions and
// limitations under the License.
//

// Warning: This code was generated by a tool.
//
// Changes to this file may cause incorrect behavior and will be lost if the
// code is regenerated.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Azure.Commands.Compute.Automation.Models;
using Microsoft.Azure.Commands.Compute.Strategies;
using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;
using Microsoft.Azure.Management.Compute;
using Microsoft.Azure.Management.Compute.Models;
using Microsoft.WindowsAzure.Commands.Utilities.Common;

namespace Microsoft.Azure.Commands.Compute.Automation
{
    [Cmdlet(VerbsCommon.New, ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "Vmss", DefaultParameterSetName = "DefaultParameter", SupportsShouldProcess = true)]
    [OutputType(typeof(PSVirtualMachineScaleSet))]
    public partial class NewAzureRmVmss : ComputeAutomationBaseCmdlet
    {
        public const string SimpleParameterSet = "SimpleParameterSet";

        public override void ExecuteCmdlet()
        {
            base.ExecuteCmdlet();

            //TODO: SImplify all these checks into an Engine class.  

            switch (ParameterSetName)
            {
                case SimpleParameterSet:
                    if (this.IsParameterBound(c => c.OrchestrationMode))
                    {
                        CheckOrchestrationModeRequirementsSimpleParameterSet();
                    }
                    else if (this.VirtualMachineScaleSet != null && this.VirtualMachineScaleSet.VirtualMachineProfile == null)
                    {
                        WriteError("The 'VirtualMachineScaleSetVMProfile' is null, indicating an orchestration mode is required. Please check the link 'aka.ms/vmProfileProperties' for more information about which parameters are part of a profile.", this.VirtualMachineScaleSet); 
                    }
                    this.StartAndWait(SimpleParameterSetExecuteCmdlet);
                    break;
                default:
                    ExecuteClientAction(() =>
                    {
                        if (ShouldProcess(this.VMScaleSetName, VerbsCommon.New))
                        {
                            string resourceGroupName = this.ResourceGroupName;
                            string vmScaleSetName = this.VMScaleSetName;
                            VirtualMachineScaleSet parameters = new VirtualMachineScaleSet();
                            ComputeAutomationAutoMapperProfile.Mapper.Map<PSVirtualMachineScaleSet, VirtualMachineScaleSet>(this.VirtualMachineScaleSet, parameters);

                            if (this.VirtualMachineScaleSet != null && this.VirtualMachineScaleSet.OrchestrationMode != null)
                            {
                                CheckOrchestrationModeRequirementsDefaultParameterSet();
                            }
                            else if (this.VirtualMachineScaleSet.VirtualMachineProfile == null)
                            {
                                WriteError("The 'VirtualMachineScaleSetVMProfile' is null, indicating an orchestration mode is required. Please check the link 'aka.ms/vmProfileProperties' for more information about which parameters are part of a profile.", this.VirtualMachineScaleSet);
                            }

                            if (parameters?.VirtualMachineProfile?.StorageProfile?.ImageReference?.Version?.ToLower() != "latest")
                            {
                                WriteWarning("You are deploying VMSS pinned to a specific image version from Azure Marketplace. \n" +
                                    "Consider using \"latest\" as the image version. This allows VMSS to auto upgrade when a newer version is available.");
                            }
                            var result = VirtualMachineScaleSetsClient.CreateOrUpdate(resourceGroupName, vmScaleSetName, parameters);
                            var psObject = new PSVirtualMachineScaleSet();
                            ComputeAutomationAutoMapperProfile.Mapper.Map<VirtualMachineScaleSet, PSVirtualMachineScaleSet>(result, psObject);
                            WriteObject(psObject);
                        }
                    });
                    break;
            }
        }

        private void CheckOrchestrationModeRequirementsDefaultParameterSet()
        {
            if (this.VirtualMachineScaleSet != null)
            {
                if (this.VirtualMachineScaleSet.VirtualMachineProfile != null)
                {
                    WriteError("The selected orchestration mode is in preview, and does not support the specified VMSS configuration. The 'VirtualMachineScaleSetVMProfile' is not null. Please check the link 'aka.ms/vmProfileProperties' for more information about which parameters are part of a profile.", this.OrchestrationMode);
                }
            }
            else if (this.VirtualMachineScaleSet.OrchestrationMode != "ScaleSetVM" & this.VirtualMachineScaleSet.OrchestrationMode != "VM")
            {
                WriteError("The selected orchestration mode is not a valid value. Please select 'VM' or 'ScaleSetVM'.", this.VirtualMachineScaleSet.OrchestrationMode);
            }
            else if (this.IsParameterBound(c => c.SinglePlacementGroup))
            {
                WriteError("The selected orchestration mode is in preview, and does not support the specified VMSS configuration. The parameter 'SinglePlacementGroup' is not allowed with an orchestration mode.", this.SinglePlacementGroup);
            }
            else if (!this.IsParameterBound(c => c.PlatformFaultDomainCount))
            {
                WriteError("The selected orchestration mode is in preview, and does not support the specified VMSS configuration. The parameter 'PlatformFaultDomainCount' is required with an orchestration mode.", this.PlatformFaultDomainCount);
            }
            else if (this.VirtualMachineScaleSet.UpgradePolicy != null | this.VirtualMachineScaleSet.Sku != null)
            {
                WriteError("The selected orchestration mode is in preview, and does not support the specified VMSS configuration. 'UpgradePolicy' and 'Sku must not be set when using an orchestration mode.'", this.OrchestrationMode);
            }
            else if (this.IsParameterBound(c => c.Zone) && this.Zone.Count > 1)
            {
                WriteError("The selected orchestration mode is in preview, and does not support the specified VMSS configuration. The parameter Zone cannot have more than one zone with an orchestration mode.", this.Zone);
            }

            
        }
        
        private void CheckOrchestrationModeRequirementsSimpleParameterSet()
        {
            // Check valid OMode values.
            if (this.OrchestrationMode != "ScaleSetVM" && this.OrchestrationMode != "VM")
            {
                WriteError("The selected orchestration mode is not a valid value. Please select 'VM' or 'ScaleSetVM'.", this.OrchestrationMode);
            }

            // Check OrchestrationMode requirements.  
            else if (this.IsParameterBound(c => c.SinglePlacementGroup))
            {
                WriteError("The selected orchestration mode is in preview, and does not support the specified VMSS configuration. The parameter 'SinglePlacementGroup' is not allowed with an orchestration mode.", this.SinglePlacementGroup);
            }
            else if (!this.IsParameterBound(c => c.PlatformFaultDomainCount))
            {
                WriteError("The selected orchestration mode is in preview, and does not support the specified VMSS configuration. The parameter 'PlatformFaultDomainCount' is required with an orchestration mode.", this.PlatformFaultDomainCount);
            }
            else if (this.IsParameterBound(c => c.Zone) && this.Zone.Count > 1)
            {
                WriteError("The selected orchestration mode is in preview, and does not support the specified VMSS configuration. The parameter 'Zone' cannot have more than one zone with an orchestration mode.", this.Zone);
            }

            //Check params directly inputted on the New-AzVmss cmdlet that would create a VMProfile. 
            else if (this.IsParameterBound(c => c.EncryptionAtHost))
            {
                WriteError("The selected orchestration mode is in preview, and does not support the specified VMSS configuration. The parameter 'EncryptionAtHost' would create a 'VirtualMachineScaleSetVMProfile' which is not compatible with an orchestration mode.", this.EncryptionAtHost);
            }
            else if (this.IsParameterBound(c => c.Priority))
            {
                WriteError("The selected orchestration mode is in preview, and does not support the specified VMSS configuration. The parameter 'Priority' would create a 'VirtualMachineScaleSetVMProfile' which is not compatible with an orchestration mode.", this.Priority);
            }
            else if (this.IsParameterBound(c => c.EvictionPolicy))
            {
                WriteError("The selected orchestration mode is in preview, and does not support the specified VMSS configuration. The parameter 'EvictionPolicy' would create a 'VirtualMachineScaleSetVMProfile' which is not compatible with an orchestration mode.", this.EvictionPolicy);
            }
            else if (this.IsParameterBound(c => c.MaxPrice))
            {
                WriteError("The selected orchestration mode is in preview, and does not support the specified VMSS configuration. The parameter 'MaxPrice' would create a 'VirtualMachineScaleSetVMProfile' which is not compatible with an orchestration mode.", this.MaxPrice);
            }
            else if (this.IsParameterBound(c => c.UpgradePolicyMode))
            {
                WriteError("The selected orchestration mode is in preview, and does not support the specified VMSS configuration.", this.UpgradePolicyMode);
            }
            else if (this.IsParameterBound(c => c.VmSize))
            {
                WriteError("The selected orchestration mode is in preview, and does not support the specified VMSS configuration.", this.VmSize);
            }
            else if (this.IsParameterBound(c => c.InstanceCount))
            {
                WriteError("The selected orchestration mode is in preview, and does not support the specified VMSS configuration.", this.InstanceCount);
            }
            else if (this.IsParameterBound(c => c.ImageName))
            {
                WriteError("The selected orchestration mode is in preview, and does not support the specified VMSS configuration.", this.ImageName);
            }
            else if (this.IsParameterBound(c => c.DataDiskSizeInGb))
            {
                WriteError("The selected orchestration mode is in preview, and does not support the specified VMSS configuration.", this.DataDiskSizeInGb);
            }
            else if (this.IsParameterBound(c => c.BackendPoolName))
            {
                WriteError("The selected orchestration mode is in preview, and does not support the specified VMSS configuration.", this.BackendPoolName);
            }
            else if (this.IsParameterBound(c => c.SubnetName))
            {
                WriteError("The selected orchestration mode is in preview, and does not support the specified VMSS configuration.", this.SubnetName);
            }
            else if (this.IsParameterBound(c => c.SubnetAddressPrefix))
            {
                WriteError("The selected orchestration mode is in preview, and does not support the specified VMSS configuration.", this.SubnetAddressPrefix);
            }
            else if (this.IsParameterBound(c => c.NatBackendPort))
            {
                WriteError("The selected orchestration mode is in preview, and does not support the specified VMSS configuration.", this.NatBackendPort);
            }
            else if (this.IsParameterBound(c => c.BackendPort))
            {
                WriteError("The selected orchestration mode is in preview, and does not support the specified VMSS configuration.", this.BackendPort);
            }
            else if (this.IsParameterBound(c => c.ScaleInPolicy))
            {
                WriteError("The selected orchestration mode is in preview, and does not support the specified VMSS configuration.", this.ScaleInPolicy);
            }

        }
        /*Not currently used, but could simplify some checks into this. 
        private void CheckOrchestrationModeOverallRequirements() //would need to access the DefaultPAram versions on teh Config object, but the SImplePAram versions on the cmdlet directly after mapping.
        {
            if (this.OrchestrationMode != "ScaleSetVM" & this.OrchestrationMode != "VM")
            {
                WriteError("The selected orchestration mode is not a valid value. Please select 'VM' or 'ScaleSetVM'.", this.OrchestrationMode);
            }
            else if (this.VirtualMachineScaleSet != null)
            {
                if (this.VirtualMachineScaleSet.VirtualMachineProfile != null)
                {
                    WriteError("The selected orchestration mode is in preview, and does not support the specified VMSS configuration. The 'VirtualMachineScaleSetVMProfile' is not null. Please check the link 'aka.ms/vmProfileProperties' for more information about which parameters are part of a profile.", this.OrchestrationMode);
                }
            }
            else if (this.IsParameterBound(c => c.SinglePlacementGroup))
            {
                WriteError("The selected orchestration mode is in preview, and does not support the specified VMSS configuration. The parameter 'SinglePlacementGroup' is not allowed with an orchestration mode.", this.SinglePlacementGroup);
            }
            else if (this.IsParameterBound(c => c.Zone))
            {
                if (this.Zone.Count > 1)
                {
                    WriteError("The selected orchestration mode is in preview, and does not support the specified VMSS configuration. The parameter Zone cannot have more than one zone with an orchestration mode.", this.Zone);
                }
            }
            else if (!this.IsParameterBound(c => c.PlatformFaultDomainCount))
            {
                WriteError("The selected orchestration mode is in preview, and does not support the specified VMSS configuration. The parameter 'PlatformFaultDomainCount' is required with an orchestration mode.", this.PlatformFaultDomainCount);
            }
        }*/

        private void WriteError(string message, params object[] args)
        {
            base.WriteError(new ErrorRecord(new Exception(String.Format(message, args)), "Error", ErrorCategory.NotSpecified, null));
        }

        [Parameter(
            ParameterSetName = "DefaultParameter",
            Position = 0,
            Mandatory = true,
            ValueFromPipelineByPropertyName = true)]
        [Parameter(
            ParameterSetName = SimpleParameterSet,
            Mandatory = false)]
        [ResourceGroupCompleter]
        public string ResourceGroupName { get; set; }

        [Parameter(
            ParameterSetName = "DefaultParameter",
            Position = 1,
            Mandatory = true,
            ValueFromPipelineByPropertyName = true)]
        [Parameter(
            ParameterSetName = SimpleParameterSet,
            Mandatory = true)]
        [Alias("Name")]
        public string VMScaleSetName { get; set; }

        [Parameter(
            ParameterSetName = "DefaultParameter",
            Position = 2,
            Mandatory = true,
            ValueFromPipeline = true)]
        public PSVirtualMachineScaleSet VirtualMachineScaleSet { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Run cmdlet in the background")]
        public SwitchParameter AsJob { get; set; }
    }
}
