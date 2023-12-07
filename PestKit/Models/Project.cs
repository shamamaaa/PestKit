﻿using System;
namespace PestKit.Models
{
	public class Project
	{
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public List<ProjectImage>? ProjectImages { get; set; }
    }
}

