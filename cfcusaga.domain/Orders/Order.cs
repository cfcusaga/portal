﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using cfcusaga.data;

namespace cfcusaga.domain.Orders
{
    public class Order
    {
        public int OrderId { get; set; }

        public System.DateTime OrderDate { get; set; }

        public string Username { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [DisplayName("First Name")]
        [StringLength(160)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [DisplayName("Last Name")]
        [StringLength(160)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(70)]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(40)]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required")]
        [StringLength(40)]
        public string State { get; set; }

        [Required(ErrorMessage = "Postal Code is required")]
        [DisplayName("Postal Code")]
        [StringLength(10)]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Country is required")]
        [StringLength(40)]
        public string Country { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [StringLength(24)]
        public string Phone { get; set; }

        //[Display(Name = "Experation Date")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //public DateTime Experation { get; set; }

        [Display(Name = "Credit Card")]
        [NotMapped]
        [DataType(DataType.CreditCard)]
        public String CreditCard { get; set; }

        [Display(Name = "Credit Card Type")]
        [NotMapped]
        public String CcType { get; set; }

        public bool SaveInfo { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "You have to agree to Consent and Release Waiver")]
        public bool IsAgreeToWaiver{ get; set; }


        [DisplayName("Email Address")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}",
            ErrorMessage = "Email is is not valid.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [ScaffoldColumn(false)]
        public decimal Total { get; set; }
        public List<OrderDetail> OrderDetails { get; set; } = new List<domain.Orders.OrderDetail>();
        public List<OrderDiscount> OrderDiscounts { get; set; } = new List<domain.Orders.OrderDiscount>();
        public string CheckNumber { get; set; }

        public decimal? CheckAmount { get; set; }

        public string Notes { get; set; }

        [Display(Name = "Check Deposited")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CheckDeposited { get; set; }

        [DisplayName("Comments")]
        public string PaymentNotes { get; set; }
        public short? OrderStatusId { get; set; }
        public string OrderStatus { get; set; }

        public string ToString(Order order)
        {
            StringBuilder bob = new StringBuilder();

            bob.Append("<p>Registration # : "+ order.OrderId +"<br>Placed at: " + order.OrderDate +" ET</p>").AppendLine();
            bob.Append("<p>Name: " + order.FirstName + " " + order.LastName + "<br>");
            bob.Append("Address: " + order.Address + " " + order.City + " " + order.State + " " + order.PostalCode + "<br>");
            bob.Append("Contact: " + order.Email + "     " + order.Phone + "</p>");
            //bob.Append("<p>Charge: " + order.CreditCard + " " + order.Experation.ToString("dd-MM-yyyy") + "</p>");
            bob.Append("<p>Check Number: " + order.CheckNumber + "</p>");

            bob.Append("<br>").AppendLine();
            bob.Append("<Table>").AppendLine();
            // Display header 
            string header = "<tr> <th>Item Name</th>" + "<th>Quantity</th>" + "<th>Price</th> <th></th> </tr>";
            bob.Append(header).AppendLine();

            String output = String.Empty;
            try
            {
                foreach (var item in order.OrderDetails)
                {
                    bob.Append("<tr>");
                    if (item.CategoryId == (int) Enums.CategoryTypeEnum.Registration)
                    {
                        output = "<td>" + item.ItemName + "(" + item.Lastname + "," + item.Firstname + ")" + "</td>" + "<td>" + item.Quantity + "</td>" + "<td>" + item.Quantity * item.UnitPrice + "</td>";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(item.TshirtSize))
                        {
                            output = "<td>" + item.ItemName + "(" + item.TshirtSize + ")" + "</td>" + "<td>" + item.Quantity + "</td>" + "<td>" + item.Quantity * item.UnitPrice + "</td>";
                        }
                        else
                        {
                            output = "<td>" + item.ItemName + "</td>" + "<td>" + item.Quantity + "</td>" + "<td>" + item.Quantity * item.UnitPrice + "</td>";
                        }
                        
                    }
                    
                    bob.Append(output).AppendLine();
                    //Console.WriteLine(output);
                    bob.Append("</tr>");
                }
                foreach (var item in order.OrderDiscounts)
                {
                    bob.Append("<tr>");
                    output = "<td>" + item.Name + "</td>" + "<td>" + item.Quantity + "</td>" + "<td> - " + item.Quantity * item.Discount + "</td>";

                    bob.Append(output).AppendLine();
                    //Console.WriteLine(output);
                    bob.Append("</tr>");
                }
            }
            catch (Exception ex)
            {
                output = "No items ordered.";
            }
            bob.Append("</Table>");
            bob.Append("<b>");
            // Display footer 
            string footer = String.Format("{0,-12}{1,12}\n",
                                          "Total", order.Total);
            bob.Append(footer).AppendLine();
            bob.Append("</b>");

            return bob.ToString();
        }
    }
}