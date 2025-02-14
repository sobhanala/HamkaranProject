### **سیستم تولید-مصرف**

این پروژه یک سیستم **تولید-مصرف** را نشان می‌دهد که در آن:
- **تولیدکننده** پیام‌ها را تولید می‌کند، آنها را سریالایز کرده و به یک API ارسال می‌کند.
- **مصرف‌کننده** پیام‌ها را از API دریافت کرده، آنها را دی‌سریالایز کرده و پردازش/ذخیره می‌کند.

این سیستم به‌گونه‌ای طراحی شده که قابلیت گسترش و add-on آسان داشته باشد و منطق تولیدکننده و مصرف‌کننده در DLLهای جداگانه پیاده‌سازی شده است.

---

## **ساختار پروژه**

### **اجزاء**
1. **ProducerPlugin**
   - پیاده‌سازی کننده اینترفیس `IProducer`.
   - پیام‌ها را تولید کرده و به رشته‌های JSON سریالایز می‌کند.
   - پیام‌ها را از طریق `ProducerLoader` به API ارسال می‌کند.

2. **ProducerLoader**
   - DLL مربوط به `ProducerPlugin` را بارگذاری می‌کند.
   - چندین thread برای تولید پیام‌ها مدیریت می‌کند.
   - با اضافه کردن یک صف مدیریت میکند که پیام ها تحت هیچ شرایطی miss نشوند حتی در صورت پایین بودن سرور 
   - پیام‌های سریالایز شده را به API ارسال می‌کند.

3. **Consumer**
   - پیاده‌سازی کننده اینترفیس `IConsumer`.
   - پیام‌ها را از API دریافت می‌کند.
   - پیام‌ها را دی‌سریالایز کرده و پردازش می‌کند.
   - پیام های را در یک فایل مینویسد

4. **LoggerLib**
   - کتابخانه‌ای برای ثبت لاگ‌ها که در سراسر پروژه برای ثبت اطلاعات، هشدارها و خطاها استفاده می‌شود.
   - این لاگر به صورت سینگلتون بوده و هر لاکی در پروژه را در مقصد خود با در فایلی  با نام shared-log میریزد

---

## **نحوه عملکرد**

### **جریان کار تولیدکننده**
1. `ProducerLoader`، DLL مربوط به `ProducerPlugin` را بارگذاری می‌کند. با استفاده از `reflection`
2. `ProducerPlugin` پیام‌ها را تولید کرده و به رشته‌های JSON سریالایز می‌کند.
3. `ProducerLoader` پیام‌های سریالایز شده را به API ارسال می‌کند.

### **جریان کار مصرف‌کننده**
1. `ConsumerLoader`، DLL مربوط به مصرف‌کننده را بارگذاری می‌کند.
2. مصرف‌کننده پیام‌های سریالایز شده را از API دریافت می‌کند.
3. مصرف‌کننده پیام‌ها را دی‌سریالایز کرده و پردازش می‌کند.

---

## **پیش‌نیازها**


2. **API Endpoint**
   - این سیستم فرض می‌کند که یک API در آدرس‌های `https://localhost:5000/api/produce` (برای تولیدکننده) و `https://localhost:5000/api/consume` (برای مصرف‌کننده) در حال اجرا است.
   - می‌توانید از ابزاری مانند [Postman](https://www.postman.com/) یا یک API ساده ASP.NET Core برای شبیه‌سازی این نقاط انتهایی استفاده کنید.


---

## **نحوه اجرا**

### **مرحله 1: ساخت فایل‌های DLL**

1. ابتدا به دایرکتوری پروژه بروید.
2. در ترمینال دستور زیر را اجرا کنید:
   ```bash
   dotnet build
   ```
3. اطمینان حاصل کنید که فایل‌های DLL ساخته شده‌اند.
پس فایل های ProducerDll , ConsumerDLl را بیلد کنید


### **مرحله 2: اجرای پروژه**

1. به دایرکتوری `hamkaran project` بروید:
   ```bash
   cd "C:\Users\Click\source\repos\hamkaran project"
   dotnet run
   ```

### **مرحله 3: اجرای تولیدکننده و مصرف‌کننده**

1. به دایرکتوری `ProducerLoader` بروید و اجرا کنید:
   ```bash
   cd ProducerLoader
   dotnet run
   ```
2. به دایرکتوری `ConsumerLoader` بروید و اجرا کنید:
   ```bash
   cd ConsumerLoader
   dotnet run
   ```

حالا سیستم تولید و مصرف آماده به کار است.



## **پیکربندی**

### **تنظیمات تولیدکننده**
می‌توانید `ProducerPlugin` را با استفاده از صفت `ProducerSettings` پیکربندی کنید:
```csharp
[ProducerSettings(ThreadCount = 3, RetryCount = 5, MessageIntervalMs = 1000)]
public class ProducerPlugin : IProducer
{
    // پیاده‌سازی
}
```
- **ThreadCount**: تعداد نخ‌های تولیدکننده.
- **RetryCount**: تعداد تلاش‌ها برای ارسال پیام‌های ناموفق.
- **MessageIntervalMs**: تأخیر بین تولید پیام‌ها (بر حسب میلی‌ثانیه).

### **تنظیمات مصرف‌کننده**
مصرف‌کننده را می‌توان با استفاده از صفت `ConsumerAttribute` پیکربندی کرد:
```csharp
[Consumer(ThreadCount = 3, RetryCount = 5)]
public class ExampleConsumer : IConsumer
{
    // پیاده‌سازی
}
```
- **ThreadCount**: تعداد های مصرف‌کننده.
- **RetryCount**: تعداد تلاش‌ها برای پیام‌های ناموفق.

---

## **لاگ‌ها**

سیستم از `LoggerLib` برای ثبت لاگ‌ها استفاده می‌کند. لاگ‌ها در کنسول نوشته می‌شوند و در صورت نیاز می‌توان آنها را به یک فایل هدایت کرد.

### **سطوح لاگ**
- **اطلاعات**: پیام‌های عمومی در مورد عملکرد.
- **هشدار**: مسائل غیر بحرانی.
- **خطا**: خطاهای بحرانی که نیاز به توجه دارند.

---

## **خروجی نمونه**

### **لاگ‌های تولیدکننده**
```
2025-02-14 15:34:32 [INFO] Producer loaded with 3 threads, 5 retries, and 1000ms interval.
2025-02-14 15:34:33 [INFO] [Thread 1] Produced: {"Id":1,"Content":"ProducerPlugin Message 1"}
2025-02-14 15:34:34 [INFO] [Thread 2] Produced: {"Id":2,"Content":"ProducerPlugin Message 2"}
```

### **لاگ‌های مصرف‌کننده**
```
2025-02-14 15:34:32 [INFO] Producer loaded with 3 threads, 5 retries, and 1000ms interval.
2025-02-14 15:34:33 [INFO] [Thread 1] Produced: {"Id":1,"Content":"ProducerPlugin Message 1"}
2025-02-14 15:34:34 [INFO] [Thread 2] Produced: {"Id":2,"Content":"ProducerPlugin Message 2"}
```

### **پیام های مصرف شده در فایل ذخیره شده **
```
2/14/2025 5:48:39 PM _messageCounter=9 message=Hello from ProducerPlugin! 
2/14/2025 5:48:39 PM _messageCounter=10 message=Hello from ProducerPlugin! 
2/14/2025 5:48:39 PM _messageCounter=12 message=Hello from ProducerPlugin! 
2/14/2025 5:48:39 PM _messageCounter=11 message=Hello from ProducerPlugin! 
2/14/2025 5:48:39 PM _messageCounter=13 message=Hello from ProducerPlugin! 
2/14/2025 5:48:39 PM _messageCounter=14 message=Hello from ProducerPlugin! 
2/14/2025 5:48:39 PM _messageCounter=15 message=Hello from ProducerPlugin! 
2/14/2025 5:48:40 PM _messageCounter=16 message=Hello from ProducerPlugin! 
2/14/2025 5:48:40 PM _messageCounter=17 message=Hello from ProducerPlugin! 
2/14/2025 5:48:40 PM _messageCounter=18 message=Hello from ProducerPlugin! 
2/14/2025 5:48:40 PM _messageCounter=19 message=Hello from ProducerPlugin! 
2/14/2025 5:48:40 PM _messageCounter=20 message=Hello from ProducerPlugin! 
```



### **فایل نگهداری بروکر**




```
{"_messageCounter":53,"message":"Hello from ProducerPlugin!"}
{"_messageCounter":54,"message":"Hello from ProducerPlugin!"}
{"_messageCounter":55,"message":"Hello from ProducerPlugin!"}
{"_messageCounter":57,"message":"Hello from ProducerPlugin!"}
{"_messageCounter":56,"message":"Hello from ProducerPlugin!"}
{"_messageCounter":58,"message":"Hello from ProducerPlugin!"}
{"_messageCounter":59,"message":"Hello from ProducerPlugin!"}
{"_messageCounter":61,"message":"Hello from ProducerPlugin!"}
{"_messageCounter":60,"message":"Hello from ProducerPlugin!"}
{"_messageCounter":62,"message":"Hello from ProducerPlugin!"}
{"_messageCounter":64,"message":"Hello from ProducerPlugin!"}
{"_messageCounter":63,"message":"Hello from ProducerPlugin!"}
{"_messageCounter":65,"message":"Hello from ProducerPlugin!"}
{"_messageCounter":67,"message":"Hello from ProducerPlugin!"}
{"_messageCounter":66,"message":"Hello from ProducerPlugin!"}
{"_messageCounter":68,"message":"Hello from ProducerPlugin!"}
{"_messageCounter":70,"message":"Hello from ProducerPlugin!"}
{"_messageCounter":69,"message":"Hello from ProducerPlugin!"}
{"_messageCounter":71,"message":"Hello from ProducerPlugin!"}
{"_messageCounter":72,"message":"Hello from ProducerPlugin!"}
{"_messageCounter":73,"message":"Hello from ProducerPlugin!"}

```
---

## 




--- 
---

