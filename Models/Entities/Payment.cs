using System;
using Microsoft.AspNetCore.Identity;

namespace MarvelWebApp.Models
{
    enum PaymentStatusType {
        Successful,
        Failed,
        Error
    } 
    enum PaymentMethodType {
        Debt,
        Visa,
        MasterCard
    }
    public class Payment 
    {
        public int PaymentID {get; set;}
        public int OrderID {get; set;}
                public Order Order { get; set; }

        public DateTime PaymentDate {get; set;}
        public decimal PaymentAmount {get; set;}

        public int TransactionID {get; set;}
        // public PaymentStatusType PaymentStatus {get; set;}
        // public PaymentMethodType PaymentMethod {get; set;}

    }
}