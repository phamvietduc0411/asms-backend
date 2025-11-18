using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Container
{
    public class PlaceContainerRequest
    {
        public string ContainerCode {  get; set; }  
        public string FloorCode { get; set; }   
        public int Layer {  get; set; } 
        public int SerialNumber { get; set; }

        public string? OrderCode { get; set; }     
        public string? PerformedBy { get; set; }
        public bool RequiresRearrangement { get; set; } 
        public string? RearrangeContainerCode {  get; set; }

    }
}
