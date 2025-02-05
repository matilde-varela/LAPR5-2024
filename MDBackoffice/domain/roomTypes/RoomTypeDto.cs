using System;
using System.Collections.Generic;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.RoomTypes
{
    public class RoomTypeDto
    {
        public string Code { get; set; }
        public string Designation { get; set; }
        public string Description { get; set; }

        public RoomTypeDto(string code, string designation, string description)
        {
            Code = code;
            Designation = designation;
            Description = description;

        }
    }
}