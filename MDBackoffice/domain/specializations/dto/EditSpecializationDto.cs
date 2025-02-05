using System;


namespace MDBackoffice.Domain.Specializations
{
    public class EditSpecializationDto
    {
        public string Code { get; set; }
        public string? Denomination { get; set; }
        public string? Description { get; set; }
        public EditSpecializationDto(string code, string denomination, string description)
        {
            Code = code;
            Denomination = denomination;
            Description = description;
        }
    }
}