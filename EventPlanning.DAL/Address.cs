using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EventPlanning.DAL
{
    public class Address
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public int? PostalCode { get; set; }

        public string FullAddress
        {
            get
            {
                var s = new StringBuilder();

                if (AddressLine1 != null)
                {
                    s.Append(AddressLine1).AppendLine();
                }

                if (AddressLine2 != null)
                {
                    s.Append(AddressLine2).AppendLine();
                }

                if (Province != null)
                {
                    s.Append(Province).AppendLine();
                }

                if (City != null)
                {
                    s.Append(City).AppendLine();
                }

                if (PostalCode != null)
                {
                    s.Append(PostalCode).AppendLine();
                }

                if (Country != null)
                {
                    s.Append(Country).AppendLine();
                }

                return s.ToString();
            }
        }
    }
}
