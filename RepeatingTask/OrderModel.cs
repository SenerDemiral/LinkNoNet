namespace RepeatingTask;

public class OrderModel
{
    public int id { get; set; }
    public string customerFirstname { get; set; }
    public string customerSurname { get; set; }
    public string customerEmail { get; set; }
    public string customerPhone { get; set; }
    public string paymentTypeName { get; set; }
    public string paymentProviderCode { get; set; }
    public string paymentProviderName { get; set; }
    public string paymentGatewayCode { get; set; }
    public string paymentGatewayName { get; set; }
    public string bankName { get; set; }
    public string clientIp { get; set; }
    public string userAgent { get; set; }
    public string currency { get; set; }
    public string currencyRates { get; set; }
    public float amount { get; set; }
    public int couponDiscount { get; set; }
    public float taxAmount { get; set; }
    public int promotionDiscount { get; set; }
    public int generalAmount { get; set; }
    public int shippingAmount { get; set; }
    public int additionalServiceAmount { get; set; }
    public int finalAmount { get; set; }
    public int sumOfGainedPoints { get; set; }
    public int installment { get; set; }
    public int installmentRate { get; set; }
    public int extraInstallment { get; set; }
    public string transactionId { get; set; }
    public int hasUserNote { get; set; }
    public string status { get; set; }
    public string paymentStatus { get; set; }
    public string errorMessage { get; set; }
    public string deviceType { get; set; }
    public string referrer { get; set; }
    public int invoicePrintCount { get; set; }
    public int useGiftPackage { get; set; }
    public string giftNote { get; set; }
    public string memberGroupName { get; set; }
    public int usePromotion { get; set; }
    public string shippingProviderCode { get; set; }
    public string shippingProviderName { get; set; }
    public string shippingCompanyName { get; set; }
    public string shippingPaymentType { get; set; }
    public string shippingTrackingCode { get; set; }
    public string source { get; set; }
    public DateTime createdAt { get; set; }
    public DateTime updatedAt { get; set; }
    public Maillist maillist { get; set; }
    public Member member { get; set; }
    public OrderDetail[] orderDetails { get; set; }
    public OrderItem[] orderItems { get; set; }
    public ShippingAddress shippingAddress { get; set; }
    public BillingAddress billingAddress { get; set; }
}

public class Maillist
{
    public int id { get; set; }
    public string name { get; set; }
    public string email { get; set; }
    public MaillistGroup maillistGroup { get; set; }
}

public class MaillistGroup
{
    public int id { get; set; }
    public string name { get; set; }
}

public class Member
{
    public int id { get; set; }
    public string firstname { get; set; }
    public string surname { get; set; }
    public string email { get; set; }
    public string gender { get; set; }
    public string phoneNumber { get; set; }
    public string mobilePhoneNumber { get; set; }
    public string address { get; set; }
    public string status { get; set; }
    public MemberGroup memberGroup { get; set; }
}

public class MemberGroup
{
    public int id { get; set; }
    public string name { get; set; }
}

public class ShippingAddress
{
    public int id { get; set; }
    public string firstname { get; set; }
    public string surname { get; set; }
    public string country { get; set; }
    public string location { get; set; }
    public string subLocation { get; set; }
    public string address { get; set; }
    public string phoneNumber { get; set; }
    public string mobilePhoneNumber { get; set; }
}

public class BillingAddress
{
    public int id { get; set; }
    public string firstname { get; set; }
    public string surname { get; set; }
    public string country { get; set; }
    public string location { get; set; }
    public string subLocation { get; set; }
    public string address { get; set; }
    public string phoneNumber { get; set; }
    public string mobilePhoneNumber { get; set; }
    public string invoiceType { get; set; }
    public string taxNo { get; set; }
    public string taxOffice { get; set; }
    public string identityRegistrationNumber { get; set; }
}

public class OrderDetail
{
    public int id { get; set; }
    public string varKey { get; set; }
    public string varValue { get; set; }
}

public class OrderItem
{
    public int id { get; set; }
    public string productName { get; set; }
    public string productSku { get; set; }
    public string productBarcode { get; set; }
    public int productPrice { get; set; }
    public string productCurrency { get; set; }
    public int productQuantity { get; set; }
    public int productTax { get; set; }
    public int productDiscount { get; set; }
    public int productMoneyOrderDiscount { get; set; }
    public float productWeight { get; set; }
    public string productStockTypeLabel { get; set; }
    public int isProductPromotioned { get; set; }
    public int discount { get; set; }
    public Product product { get; set; }
    public OrderItemCustomizations[] orderItemCustomizations { get; set; }
    public OrderItemSubscription orderItemSubscription { get; set; }
}

public class Product
{
    public int id { get; set; }
}

public class OrderItemSubscription
{
    public int id { get; set; }
}

public class OrderItemCustomizations
{
    public int id { get; set; }
    public int productCustomizationGroupId { get; set; }
    public string productCustomizationGroupName { get; set; }
    public int productCustomizationGroupSortOrder { get; set; }
    public int productCustomizationFieldId { get; set; }
    public string productCustomizationFieldType { get; set; }
    public string productCustomizationFieldName { get; set; }
    public string productCustomizationFieldValue { get; set; }
    public int cartItemAttributeId { get; set; }
}
/*
status
------
waiting_for_approval:   Onay Bekliyor
approved:               Onaylandı
fulfilled:              Kargoya Verildi
cancelled:              İptal Edildi
delivered:              Teslim Edildi       -----
on_accumulation:        Tedarik Sürecinde
waiting_for_payment:    Ödeme Bekleniyor
being_prepared:         Hazırlanıyor
refunded:               İade Edildi         -----
personal_status_1:      Kişisel Sipariş Durumu 1
personal_status_2:      Kişisel Sipariş Durumu 2
personal_status_3:      Kişisel Sipariş Durumu 3
deleted:                Silindi

paymentStatus
-------------
success:                Başarılı
in_transaction:         Sonuçlanmamış Ödemeler
failed:                 Hatalı Ödemeler

Query strings
startDate:      createdAt değeri için başlangıç tarihi. string (yyyy-mm-dd hh:mm:ss)
endDate:        createdAt değeri için bitiş tarihi.     string (yyyy-mm-dd hh:mm:ss)
startUpdatedAt: updatedAt değeri için başlangıç tarihi  string (yyyy-mm-dd hh:mm:ss)
endUpdatedAt:   updatedAt değeri için bitiş tarihi      string (yyyy-mm-dd hh:mm:ss)
*/