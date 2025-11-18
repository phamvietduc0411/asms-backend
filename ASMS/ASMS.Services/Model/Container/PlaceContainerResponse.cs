using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Container
{
    public class PlaceContainerResponse
    {
        public bool Success { get; set; }   
        public string Message {  get; set; }
        public string ContainerCode {  get; set; }
        public string FloorCode {  get; set; }  
        public int? Layer {  get; set; }
        public int? SerialNumber { get; set; }  
    }
}
