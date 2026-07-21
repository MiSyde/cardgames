using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cards.Models
{
    public interface IHand
    {
        int Id { get; set; }
        bool IsActive { get; set; }
        bool IsSplit { get; }
        int PlayerId { get; }
    }
}
