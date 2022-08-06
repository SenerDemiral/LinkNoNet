namespace TntMud.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

[Table("UT")]
public class UT
{
    [Key]
    public int UtID { get; set; }

    [StringLength(20)]
    public string Name { get; set; }
    
    [StringLength(1)]
    public string Typ { get; set; }
}

