using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Telephony.EwsdModel.Table;

public class EwsdFile
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    [Required]
    public string Path { get; set; }
    
    [Required]
    public string Name { get; set; }

    [InverseProperty("File")]
    public ICollection<EwsdFileParsingTask> FileParsingTasks { get; set; }
}