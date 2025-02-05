using System.ComponentModel;

public enum ChangeType
{
    [Description("Deletion")]
    Deletion = 1,
    [Description("Creation")]    
    Creation = 2,    
    [Description("Edit")]
    Edit = 3,   

}
