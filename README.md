_Logistic GUI for NFC-IWMS_

**Introduction**

The NFC Card Reader Solution is a software project that allows interaction with NFC (Near Field Communication) cards using a compatible card reader. The solution consists of a Product Controller and associated services, which provide APIs for managing products, and an NFCReader class, responsible for handling card-related operations.

**Product Controller and Services**

The Product Controller is the core of the solution, offering various HTTP endpoints for managing products. Here are the main functions provided by the controller:

- GET /api/products: Retrieves a list of products.
- GET /api/products/{id}: Retrieves a specific product by its unique identifier.
- POST /api/products: Creates a new product.
- PUT /api/products/{id}: Updates an existing product.
- DELETE /api/products/{id}: Deletes a product.

These endpoints enable users to perform essential CRUD (Create, Read, Update, Delete) operations on products.

**NFCReader Class**

The NFCReader class is a critical component of the NFC Card Reader Solution. It allows communication with NFC cards through a compatible card reader. Here's how it works:

Card Insertion and Removal Handling:

The NFCReader class has two events:

- CardInserted: Triggered when a card is inserted into the card reader.
- CardRemoved: Triggered when a card is removed from the card reader.

These events are essential for detecting card-related actions and can be subscribed to in your application. For instance, you can use them to trigger specific actions when a card is inserted or removed, like reading card data or updating user authentication status.

**Initialization and Usage**

To use the NFCReader, you typically follow these steps:

1. Initialization: Create an instance of the NFCReader class.
   ```csharp
   NFCReader NFC = new NFCReader();
2. Connecting: Establish a connection to an NFC card reader.
    ```csharp
    NFC.Connect();
3. Disconnecting: Safely disconnect from the NFC card reader.
    ```csharp
    NFC.Disconnect();

**Event Handling**
1. Card Inserted Event: Subscribe to the CardInserted event to perform actions when a card is inserted.
    ```csharp
    NFC.CardInserted += new NFCReader.CardEventHandler(Card_Inserted);
2. Card Ejected Event: Subscribe to the CardEjected event to perform actions when a card is ejected.
   ```csharp
    NFC.CardEjected += new NFCReader.CardEventHandler(Card_Ejected);
3. Enabling Event Watching: Activate event monitoring to respond to card insertion and ejection.
   ```csharp
    NFC.Watch();

  
**Read, Write, Authorize**
1. Authorization (Automatically Handled): NFC card authorization is automatically handled by the read and write functions.
2. Reading Data: Read data from an NFC card.
   ```csharp
    NFC.ReadBlock("2");
3. Writing Data: Write data to an NFC card.
   ```csharp
    NFC.WriteBlock("Some string data that will not exceed block limit", "2");

**Reader List, Card UID**
1. Card UID: Retrieve the unique identifier (UID) of an NFC card.
   ```csharp
    NFC.GetCardUID();
2. Available Readers: Get a list of available NFC card readers.
   ```csharp
    NFC.GetReadersList();

**Example: Inserted and Ejected Event Usage**
```csharp
      public void Card_Inserted()
      {
        try
        {
          if (NFC.Connect())
          {
              // Do stuff like NFC.GetCardUID(); ...
          }
          else
          {
              // Give an error message about the connection...
          }
        }
        catch (Exception ex)
        {
          this.SetStatusText("Error: An issue occurred. Please try again.", false);
        }
      }
      
      public void Card_Ejected()
      {
         // Do stuff...
         NFC.Disconnect();
      }
```
**Conclusion**

The NFC Card Reader Solution offers a convenient way to interact with NFC cards through a card reader. The Product Controller and services allow for efficient management of products, while the NFCReader class simplifies the integration of NFC card functionality into your application.




