using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class StorageType
{
    public int StorageTypeId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Storage> Storages { get; set; } = new List<Storage>();

    public virtual ICollection<WorkflowTemplate> WorkflowTemplates { get; set; } = new List<WorkflowTemplate>();
}
