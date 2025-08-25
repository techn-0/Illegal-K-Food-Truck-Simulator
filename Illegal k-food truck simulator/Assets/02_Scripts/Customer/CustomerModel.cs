using System.Collections.Generic;
using UnityEngine;

public class CustomerModel
{
    public string CustomerId { get; private set; } // 손님 ID
    public string CurrentOrder { get; private set; } // 현재 주문한 레시피 ID
    public bool IsServed { get; private set; } // 주문 완료 여부

    public CustomerModel(string customerId, string initialOrder)
    {
        CustomerId = customerId;
        CurrentOrder = initialOrder;
        IsServed = false;
    }

    public void ServeOrder()
    {
        IsServed = true;
    }
}
