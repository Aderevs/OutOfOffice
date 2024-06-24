﻿using Microsoft.AspNetCore.Mvc.Rendering;
using OutOfOffice.DbLogic;
using System.ComponentModel.DataAnnotations;

namespace OutOfOffice.Models
{
    public class EmployeeBinding
    {
        public EmployeeBinding() { }
        public EmployeeBinding(List<Employee> allHRs)
        {
            Options = [];
            foreach (var hr in allHRs)
            {
                Options.Add(new(hr.FullName, hr.ID.ToString()));
            }
        }
        [Required]
        public string FullName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public Subdivision Subdivision { get; set; }

        [Required]
        public Position Position { get; set; }

        [Required]
        public int OutOfOfficeBalance { get; set; }

        public string? PeoplePartnerId { get; set; }

        public List<SelectListItem> Options { get; set; }
    }
}
