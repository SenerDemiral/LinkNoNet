namespace TntMud.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

[Table("UT")]
public sealed class UT
{
    [Key]
    public int UtID { get; set; }

    [StringLength(20)]
    public string Name { get; set; }
    
    [StringLength(1)]
    public string Typ { get; set; }


    public string Lbls
    {
        get { return String.Join(',', LblAry); }
        set { populate(value); }
    }

    public int[] LblAry { get; set; }

    public bool fndAnd = false;

    public void populate(string lbls)
    {
        int mxLbl = 20; // Max labl for usr. ushort: 65_535 different Lbl

        string[] stra = lbls.Split(',', mxLbl, StringSplitOptions.RemoveEmptyEntries);
        LblAry = new int[stra.Length];

        int i = 0;
        foreach (string str in stra)
        {
            LblAry[i++] = ushort.Parse(str);
        }
    }

}

