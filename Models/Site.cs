using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Net;

namespace Hackathon.Models
{
    public class Site
    {
        [Key]
        public int Site_PK { get; set; }
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        public string Site_Code { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Site_Name { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        //public Point Location { get; set; }

        /// <summary>
        /// Gets or sets the Address.
        /// </summary>
        public string Site_Address_1 { get; set; }

        /// <summary>
        /// Gets or sets the IsAddressUpdate.
        /// </summary>
        //public bool IsAddressUpdated { get; set; }

        /// <summary>
        /// Gets or sets the entity id.
        /// </summary>
        public string Site_Status { get; set; }

        /// <summary>
        /// Gets or sets the region code.
        /// </summary>
        public decimal Site_Longitude { get; set; }

        public decimal Site_Latitude { get; set; }

    }
}
