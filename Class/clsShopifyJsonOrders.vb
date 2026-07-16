Imports Newtonsoft.Json

Public Class clsShopifyJsonOrders
    Public Class ClientDetails
        Public Property accept_language As String
        Public Property browser_height As Object
        Public Property browser_ip As String
        Public Property browser_width As Object
        Public Property session_hash As Object
        Public Property user_agent As String
    End Class

    Public Class ShopMoney
        Public Property amount As String
        Public Property currency_code As String
    End Class

    Public Class PresentmentMoney
        Public Property amount As String
        Public Property currency_code As String
    End Class

    Public Class CurrentSubtotalPriceSet
        Public Property shop_money As ShopMoney
        Public Property presentment_money As PresentmentMoney
    End Class

    Public Class CurrentTotalDiscountsSet
        Public Property shop_money As ShopMoney
        Public Property presentment_money As PresentmentMoney
    End Class

    Public Class CurrentTotalPriceSet
        Public Property shop_money As ShopMoney
        Public Property presentment_money As PresentmentMoney
    End Class

    Public Class CurrentTotalTaxSet
        Public Property shop_money As ShopMoney
        Public Property presentment_money As PresentmentMoney
    End Class

    Public Class NoteAttribute
        Public Property name As String
        Public Property value As String
    End Class

    Public Class SubtotalPriceSet
        Public Property shop_money As ShopMoney
        Public Property presentment_money As PresentmentMoney
    End Class

    Public Class PriceSet
        Public Property shop_money As ShopMoney
        Public Property presentment_money As PresentmentMoney
    End Class

    ' Single TaxLine definition used everywhere
    Public Class TaxLine
        Public Property channel_liable As Boolean
        Public Property price As String
        Public Property price_set As PriceSet
        Public Property rate As Double
        Public Property title As String
    End Class

    Public Class TotalCashRoundingPaymentAdjustmentSet
        Public Property shop_money As ShopMoney
        Public Property presentment_money As PresentmentMoney
    End Class

    Public Class TotalCashRoundingRefundAdjustmentSet
        Public Property shop_money As ShopMoney
        Public Property presentment_money As PresentmentMoney
    End Class

    Public Class TotalDiscountsSet
        Public Property shop_money As ShopMoney
        Public Property presentment_money As PresentmentMoney
    End Class

    Public Class TotalLineItemsPriceSet
        Public Property shop_money As ShopMoney
        Public Property presentment_money As PresentmentMoney
    End Class

    Public Class TotalPriceSet
        Public Property shop_money As ShopMoney
        Public Property presentment_money As PresentmentMoney
    End Class

    Public Class TotalShippingPriceSet
        Public Property shop_money As ShopMoney
        Public Property presentment_money As PresentmentMoney
    End Class

    Public Class TotalTaxSet
        Public Property shop_money As ShopMoney
        Public Property presentment_money As PresentmentMoney
    End Class

    Public Class BillingAddress
        Public Property first_name As String
        Public Property address1 As String
        Public Property phone As String
        Public Property city As String
        Public Property zip As String
        Public Property province As Object
        Public Property country As String
        Public Property last_name As String
        Public Property address2 As String
        Public Property company As Object
        Public Property latitude As Double
        Public Property longitude As Double
        Public Property name As String
        Public Property country_code As String
        Public Property province_code As Object
    End Class

    Public Class EmailMarketingConsent
        Public Property state As String
        Public Property opt_in_level As String
        Public Property consent_updated_at As DateTime?
    End Class

    Public Class DefaultAddress
        Public Property id As Object
        Public Property customer_id As Object
        Public Property first_name As String
        Public Property last_name As String
        Public Property company As Object
        Public Property address1 As String
        Public Property address2 As String
        Public Property city As String
        Public Property province As Object
        Public Property country As String
        Public Property zip As String
        Public Property phone As String
        Public Property name As String
        Public Property province_code As Object
        Public Property country_code As String
        Public Property country_name As String

        <JsonProperty("default")>
        Public Property defaults As Boolean
    End Class

    Public Class Customer
        Public Property id As Object
        Public Property created_at As DateTime?
        Public Property updated_at As DateTime?
        Public Property first_name As String
        Public Property last_name As String
        Public Property state As String
        Public Property note As Object
        Public Property verified_email As Boolean
        Public Property multipass_identifier As Object
        Public Property tax_exempt As Boolean
        Public Property email_marketing_consent As EmailMarketingConsent
        Public Property sms_marketing_consent As Object
        Public Property tags As String
        Public Property email As String
        Public Property phone As Object
        Public Property currency As String
        Public Property tax_exemptions As Object()
        Public Property admin_graphql_api_id As String
        Public Property default_address As DefaultAddress
    End Class

    Public Class OriginAddress
    End Class

    Public Class Receipt
    End Class

    Public Class TotalDiscountSet
        Public Property shop_money As ShopMoney
        Public Property presentment_money As PresentmentMoney
    End Class

    ' Single LineItem definition used everywhere (includes tax_lines typed to TaxLine)
    Public Class LineItem
        Public Property id As Object
        Public Property admin_graphql_api_id As String
        Public Property attributed_staffs As Object()
        Public Property current_quantity As Integer
        Public Property fulfillable_quantity As Integer
        Public Property fulfillment_service As String
        Public Property fulfillment_status As String
        Public Property gift_card As Boolean
        Public Property grams As Integer
        Public Property name As String
        Public Property price As String
        Public Property price_set As PriceSet
        Public Property product_exists As Boolean
        Public Property product_id As Object
        Public Property properties As Object()
        Public Property quantity As Integer
        Public Property requires_shipping As Boolean
        Public Property sku As String
        Public Property taxable As Boolean
        Public Property title As String
        Public Property total_discount As String
        Public Property total_discount_set As TotalDiscountSet
        Public Property variant_id As Object
        Public Property variant_inventory_management As String
        Public Property variant_title As String
        Public Property vendor As String
        Public Property tax_lines As TaxLine()
        Public Property duties As Object()
        Public Property discount_allocations As Object()
    End Class

    Public Class Fulfillment
        ' Made numeric fields nullable to avoid errors when JSON contains null
        Public Property id As Long?
        Public Property admin_graphql_api_id As String
        Public Property created_at As DateTime?
        Public Property location_id As Long?
        Public Property name As String
        Public Property order_id As Long?
        Public Property origin_address As OriginAddress
        Public Property receipt As Receipt
        Public Property service As String
        Public Property shipment_status As String
        Public Property status As String
        Public Property tracking_company As Object
        Public Property tracking_number As String
        Public Property tracking_numbers As String()
        Public Property tracking_url As String
        Public Property tracking_urls As String()
        Public Property updated_at As DateTime?
        Public Property line_items As LineItem()
    End Class

    Public Class ShippingAddress
        Public Property first_name As String
        Public Property address1 As String
        Public Property phone As String
        Public Property city As String
        Public Property zip As String
        Public Property province As Object
        Public Property country As String
        Public Property last_name As String
        Public Property address2 As String
        Public Property company As Object
        Public Property latitude As Double
        Public Property longitude As Double
        Public Property name As String
        Public Property country_code As String
        Public Property province_code As Object
    End Class

    Public Class DiscountedPriceSet
        Public Property shop_money As ShopMoney
        Public Property presentment_money As PresentmentMoney
    End Class

    Public Class ShippingLine
        Public Property id As Object
        Public Property carrier_identifier As Object
        Public Property code As String
        Public Property discounted_price As String
        Public Property discounted_price_set As DiscountedPriceSet
        Public Property is_removed As Boolean
        Public Property phone As Object
        Public Property price As String
        Public Property price_set As PriceSet
        Public Property requested_fulfillment_service_id As Object
        Public Property source As String
        Public Property title As String
        Public Property tax_lines As TaxLine()
        Public Property discount_allocations As Object()
    End Class

    Public Class Order
        Public Property id As Object
        Public Property admin_graphql_api_id As String
        Public Property app_id As Integer
        Public Property browser_ip As String
        Public Property buyer_accepts_marketing As Boolean
        Public Property cancel_reason As Object
        Public Property cancelled_at As DateTime?
        Public Property cart_token As String
        Public Property checkout_id As Object
        Public Property checkout_token As String
        Public Property client_details As ClientDetails
        Public Property closed_at As Object
        Public Property company As Object
        Public Property confirmation_number As String
        Public Property confirmed As Boolean
        Public Property contact_email As String
        Public Property created_at As DateTime
        Public Property currency As String
        Public Property current_subtotal_price As String
        Public Property current_subtotal_price_set As CurrentSubtotalPriceSet
        Public Property current_total_additional_fees_set As Object
        Public Property current_total_discounts As String
        Public Property current_total_discounts_set As CurrentTotalDiscountsSet
        Public Property current_total_duties_set As Object
        Public Property current_total_price As String
        Public Property current_total_price_set As CurrentTotalPriceSet
        Public Property current_total_tax As String
        Public Property current_total_tax_set As CurrentTotalTaxSet
        Public Property customer_locale As String
        Public Property device_id As Object
        Public Property discount_codes As Object()
        Public Property duties_included As Boolean
        Public Property email As String
        Public Property estimated_taxes As Boolean
        Public Property financial_status As String
        Public Property fulfillment_status As String
        Public Property landing_site As String
        Public Property landing_site_ref As Object
        Public Property location_id As Object
        Public Property merchant_business_entity_id As String
        Public Property merchant_of_record_app_id As Object
        Public Property name As String
        Public Property note As Object
        Public Property note_attributes As NoteAttribute()
        Public Property number As Integer
        Public Property order_number As Integer
        Public Property order_status_url As String
        Public Property original_total_additional_fees_set As Object
        Public Property original_total_duties_set As Object
        Public Property payment_gateway_names As String()
        Public Property phone As Object
        Public Property po_number As Object
        Public Property presentment_currency As String
        Public Property processed_at As DateTime
        Public Property reference As Object
        Public Property referring_site As String
        Public Property source_identifier As Object
        Public Property source_name As String
        Public Property source_url As Object
        Public Property subtotal_price As String
        Public Property subtotal_price_set As SubtotalPriceSet
        Public Property tags As String
        Public Property tax_exempt As Boolean
        Public Property tax_lines As TaxLine()
        Public Property taxes_included As Boolean
        Public Property test As Boolean
        Public Property token As String
        Public Property total_cash_rounding_payment_adjustment_set As TotalCashRoundingPaymentAdjustmentSet
        Public Property total_cash_rounding_refund_adjustment_set As TotalCashRoundingRefundAdjustmentSet
        Public Property total_discounts As String
        Public Property total_discounts_set As TotalDiscountsSet
        Public Property total_line_items_price As String
        Public Property total_line_items_price_set As TotalLineItemsPriceSet
        Public Property total_outstanding As String
        Public Property total_price As String
        Public Property total_price_set As TotalPriceSet
        Public Property total_shipping_price_set As TotalShippingPriceSet
        Public Property total_tax As String
        Public Property total_tax_set As TotalTaxSet
        Public Property total_tip_received As String
        Public Property total_weight As Integer
        Public Property updated_at As DateTime
        Public Property user_id As Object
        Public Property billing_address As BillingAddress
        Public Property customer As Customer
        Public Property discount_applications As Object()
        Public Property fulfillments As Fulfillment()
        Public Property line_items As LineItem()
        Public Property payment_terms As Object
        Public Property refunds As Refund()
        Public Property shipping_address As ShippingAddress
        Public Property shipping_lines As ShippingLine()
    End Class

    Public Class Refund
        Public Property id As Object
        Public Property admin_graphql_api_id As String
        Public Property created_at As DateTime?
        Public Property note As String
        Public Property order_id As Object
        Public Property processed_at As DateTime?
        Public Property restock As Boolean
        Public Property total_duties_set As TotalDutiesSet
        Public Property user_id As Long?
        Public Property order_adjustments As OrderAdjustment()
        Public Property transactions As Transaction()
        Public Property refund_line_items As RefundLineItem()
        Public Property duties As Object()
    End Class

    Public Class RefundLineItem
        Public Property id As Long?
        Public Property line_item_id As Long?
        Public Property location_id As Long?
        Public Property quantity As Integer
        Public Property restock_type As String
        Public Property subtotal As Double
        Public Property subtotal_set As SubtotalSet
        Public Property total_tax As Double
        Public Property total_tax_set As TotalTaxSet
        Public Property line_item As LineItem
    End Class

    Public Class Transaction
        Public Property id As Long?
        Public Property admin_graphql_api_id As String
        Public Property amount As String
        Public Property authorization As Object
        Public Property created_at As DateTime?
        Public Property currency As String
        Public Property device_id As Object
        Public Property error_code As Object
        Public Property gateway As String
        Public Property kind As String
        Public Property location_id As Object
        Public Property message As Object
        Public Property order_id As Long?
        Public Property parent_id As Long?
        Public Property payment_id As String
        Public Property processed_at As DateTime?
        Public Property receipt As Receipt
        Public Property source_name As String
        Public Property status As String
        Public Property test As Boolean
        Public Property user_id As Long?
        Public Property payment_details As PaymentDetails
    End Class

    Public Class OrderAdjustment
        Public Property id As Object
        Public Property amount As String
        Public Property amount_set As AmountSet
        Public Property kind As String
        Public Property order_id As Object
        Public Property reason As String
        Public Property refund_id As Object
        Public Property tax_amount As String
        Public Property tax_amount_set As TaxAmountSet
    End Class

    Public Class TotalDutiesSet
        Public Property shop_money As ShopMoney
        Public Property presentment_money As PresentmentMoney
    End Class

    Public Class SubtotalSet
        Public Property shop_money As ShopMoney
        Public Property presentment_money As PresentmentMoney
    End Class

    Public Class PaymentDetails
        Public Property credit_card_bin As String
        Public Property avs_result_code As Object
        Public Property cvv_result_code As Object
        Public Property credit_card_number As String
        Public Property credit_card_company As String
        Public Property buyer_action_info As Object
        Public Property credit_card_name As String
        Public Property credit_card_wallet As String
        Public Property credit_card_expiration_month As Integer
        Public Property credit_card_expiration_year As Integer
        Public Property payment_method_name As String
    End Class

    Public Class AmountSet
        Public Property shop_money As ShopMoney
        Public Property presentment_money As PresentmentMoney
    End Class

    Public Class TaxAmountSet
        Public Property shop_money As ShopMoney
        Public Property presentment_money As PresentmentMoney
    End Class
    Public Class Orders
        Public Property orders As Order()
    End Class

End Class
