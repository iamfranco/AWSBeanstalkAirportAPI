# Airport API with AWS Elastic Beanstalk

This is a C# solution to provide a **RESTful web API** for simple airport information.

![Cloudcraft Diagram](Diagrams/Cloudcraft3D.png)

| AWS Service Involved | Purpose                                  | Free Tier Limit                                                                                                                                                                                                                                            |
| -------------------- | ---------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| DynamoDB             | Store airport information                | Free forever on free tier AWS, as long as storage is under 25GB and read/write requests under 200 million per month                                                                                                                                        |
| Elastic Beanstalk    | to serve the Web API using EC2 instances | Free **only for first 12 months** on free tier AWS. The EC2 instances that Beanstalk uses are priced at EC2 prices, so recommend using `t2.micro` or `t3.micro` EC2 instances because it is free for first 12 months at under 750 hours of usage per month |

# Instruction / Guide

This guide goes through the process of setting up the AWS services required for this application.

This guide assumes that

1. you're using [Visual Studio 2022 on Windows](https://visualstudio.microsoft.com/vs/)
2. you have [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) installed
3. you have [AWS Toolkit for Visual Studio](https://aws.amazon.com/visualstudio/) installed
4. you have an AWS account ([AWS Free Tier](https://aws.amazon.com/free) or otherwise)
5. you have cloned this repo

<details>
<summary><span style="font-size:1.2rem">Create a new IAM user with Administrator access</span></summary>

If you don't already have a IAM user with administrator access already, you should create one now.

First, login to your AWS account on the website and search for the **IAM** service on the top bar.

![IAM Search](Diagrams/screenshots/IAM/Search.png)

Click **Users** on the left menu
![IAM Home](Diagrams/screenshots/IAM/Home.png)

As we can see, we don't have any users yet, so click the **Add Users** button on the top right
![IAM AddUsers](Diagrams/screenshots/IAM/AddUsers.png)

Set the **User name** as `vsuser` and tick the **Access key - Programmatic access** checkbox because we want to access this user with our Visual Studio AWS SDK. Then click the bottom right **Next: Permissions** button.
![IAM AddUser Name](Diagrams/screenshots/IAM/AddUser_Name.png)

Go to the **Attach existing Policies** tab and tick the **administratorAccess** checkbox, this allows the `vsuser` user to have complete access for all AWS services, including creating DynamoDB database or Elastic Beanstalk. Then click the bottom right **Next: Tags** button.
![IAM AddUser Permissions](Diagrams/screenshots/IAM/AddUser_Permissions.png)

Leave the tags empty and click the **Next: Review** button.
![IAM AddUser Tag](Diagrams/screenshots/IAM/AddUser_Tag.png)

Here we can see the user being created has user name of `vsuser`, AWS access type of **Programmatic access - with an access key**, permissions summary managed policy of **AdministratorAccess**. Click **Create user** button.
![IAM AddUser Review](Diagrams/screenshots/IAM/AddUser_Review.png)

Now the new user `vsuser` has been successfully created, be sure to click the **Download .csv** button to save the credentials CSV file onto your local hard drive. We will use this CSV file later when we need to sign in to `vsuser` through the Visual Studio 2022 AWS SDK.
![IAM AddUser Created](Diagrams/screenshots/IAM/AddUser_Created.png)

Going back to the **Users** page, we see that the new user `vsuser` has indeed been successfully created.
![IAM Home After Created](Diagrams/screenshots/IAM/Home_After_Created.png)

</details>

<details>
<summary><span style="font-size:1.2rem">Visual Studio AWS Profile Sign In</span></summary>

Open up Visual Studio, if you have installed the [AWS Toolkit for Visual Studio](https://aws.amazon.com/visualstudio/) correctly, you should see this option in **View** > **AWS Explorer**

![View AWS Explorer](Diagrams/screenshots/VisualStudio/AWS_Explorer.png)

Click to open **AWS Explorer** and you should see this button (**Add AWS Credentials Profile**), click it.

![Add AWS Credentials Profile Button](Diagrams/screenshots/VisualStudio/AWS_Explorer_Add_Profile.png)

Then fill in the correct details in the pop up window:

1. Profile name, could be anything but for simplicity sake let's keep it `vsuser`
2. Import from CSV file and select the CSV file with the credentials you've downloaded in the **Create IAM user with administrator access** in the previous section
3. Set the Region to your region
4. click OK.

![New Account Profile](Diagrams/screenshots/VisualStudio/New_Account_Profile.png)

After that, the profile should be signed in to the AWS Explorer, and all the AWS services would be available through the AWS Explorer in Visual Studio.

![AWS Explorer After Profile](Diagrams/screenshots/VisualStudio/AWS_Explorer_After_Profile.png)

One thing to note is that `appsettings.Development.json` has the content:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AWS": {
    "Profile": "vsuser",
    "Region": "eu-west-2"
  }
}
```

where

- `AWS.Profile` of `"vsuser"` should be matching the profile name, and
- `AWS.Region` of `"eu-west-2"` should be matching your region

So if you chose a different profile name or region, be sure to adjust the `appsettings.Development.json` accordingly.

</details>

<details>
<summary><span style="font-size:1.2rem">Create new DynamoDB airports table </span></summary>

Now that the AWS Explorer has profile `vsuser` connected, it's time to create a new DynamoDB table for airports.

Before we begin, notice that the `Models/Airport.cs` class has the content:

```c#
using Amazon.DynamoDBv2.DataModel;

namespace AirportAPI.Models;

[DynamoDBTable("airports")]
public class Airport
{
    [DynamoDBHashKey("code")]
    public string? Code { get; set; }

    [DynamoDBProperty("name")]
    public string? Name { get; set; }

    [DynamoDBProperty("city")]
    public string? City { get; set; }
}
```

where the

```c#
[DynamoDBTable("airports")]
```

defines that the DynamoDB table should have the name `"airports"`, so when we create the DynamoDB table, it should have the name **airports**.

Also notice that the hash key name is `"code"` by this line in `Models/Airport.cs`

```c#
[DynamoDBHashKey("code")]
```

To create the **airports** table, open AWS Explorer, right click on **Amazon DynamoDB** and click **Create Table...**

![DynamoDB Create Table](Diagrams/screenshots/DynamoDB/Create_Table.png)

A window pops up for creating new DynamoDB Table:

- set the **Table Name** as **airports**
- set the **Hash Key Name** as **code**, with **Hash Key Type** as **String**
- Click **Create**

![DynamoDB Create Table Basic Settings](Diagrams/screenshots/DynamoDB/Create_Table_Basic_Settings.png)

Now after creating this new DynamoDB table, we can see it under the **Amazon DynamoDB** in the AWS Explorer

![AWS Explorer Airports Table](Diagrams/screenshots/DynamoDB/AWS_Explorer_After_Created_Table.png)

Double clicking on the **airports** table opens it up, and shows an empty table

![Airports Table Empty](Diagrams/screenshots/DynamoDB/Table_Airports.png)

Also, if open the AWS website and go to the **DynamoDB** page, you can see the **airports** table there

![Airports Table Empty](Diagrams/screenshots/DynamoDB/DynamoDB_Website.png)

</details>

<details>
<summary><span style="font-size:1.2rem">Test Run the Web API locally</span></summary>

Now that DynamoDB table **airports** has been created, you should now be able to run the Web API locally and test out the endpoints on swagger.

First, click on the **AirportAPI** button to run it in debug mode.

![Run AirportAPI button](Diagrams/screenshots/TestLocally/RunAirportAPI_Button.png)

It should open up a new page for **Swagger**, listing all the endpoints.

![AirportAPI Swagger](Diagrams/screenshots/TestLocally/AirportAPI_Swagger.png)

Sending a **GET** request to the `/api/Airport` endpoint would initially respond with **empty list of airports**, `[]`, with status code of `200`

![AirportAPI Swagger](Diagrams/screenshots/TestLocally/Swagger_GetAllAirports.png)

> ## If it responds with error
>
> But if it responds with error message instead, then it's likely because some AWS configuration weren't set properly. It could be:
>
> - The AWS user (say `vsuser`) didn't have permission to access DynamoDB. Go to **IAM** service page on AWS and add the permission to have **AdministratorAccess** which would certainly grant it access to DynamoDB.
> - The AWS user did have permission to access DynamoDB, but it was not configured correctly in `appsettings.Development.json`. Go to `appsettings.Development.json` and make sure the attributes under `"AWS"` were set appropriately for the user profile, it could be the `"Profile"` not matching or the `"Region"` not matching.
> - The DynamoDB table name not matching the table name of `"airports"` specified in `Models/Airport.cs`.
>
> Or it could be something else entirely, try to resolve that before proceeding to next step.

Next, we could send a **POST** request to the `/api/Airport` endpoint to create a new airport in the DynamoDB **airports** table.

For example, sending a **POST** request with request body

```json
{
  "code": "MAN",
  "name": "Manchester Airport",
  "city": "Manchester"
}
```

would create this new airport item in the **airports** table
![DynamoDB new airport on website](Diagrams/screenshots/TestLocally/DynamoDB_new_airport.png)

Next, you can try all the endpoints, they should all work as expected.

| action   | endpoint              | what it does                                                                                     |
| -------- | --------------------- | ------------------------------------------------------------------------------------------------ |
| `GET`    | `/api/Airport`        | Get a list of all airports                                                                       |
| `GET`    | `/api/Airport/{code}` | Get airport with matching airport code to `{code}`                                               |
| `POST`   | `/api/Airport`        | Create new airport, where request body should contain info (in `json` format) of new airport     |
| `PUT`    | `/api/Airport`        | Update airport, where request body should contain info (in `json` format) of the updated airport |
| `DELETE` | `/api/Airport/{code}` | Delete airport with matching airport code to `{code}`                                            |

</details>

<details>
<summary><span style="font-size:1.2rem">Publish to AWS Elastic Beanstalk</span></summary>

Now that we have tested the endpoints locally, we're ready to publish this web API onto AWS Elastic Beanstalk.

On Solution Explorer, we see the **AirportAPI** project,

![Publish to AWS](Diagrams/screenshots/Beanstalk/Solution_Explorer.png)

right click on the **AirportAPI** project and click **Publish to AWS**

![Publish to AWS](Diagrams/screenshots/Beanstalk/Solution_Explorer_Publish_To_AWS.png)

This opens up the **Publish to AWS: AirportAPI** page, select **ASP.NET Core App to AWS Elastic Beanstalk on Linux** and click the **Edit settings** button

![Publish to AWS](Diagrams/screenshots/Beanstalk/Publish_To_AWS.png)

Most of the default settings are correct, just change 2 things:

1. **Environment Type** to **Load Balancer**, this allows elastic beanstalk to use multiple EC2 instances to serve up the web API, where the EC2 instances will be placed in some **auto scaling group** to scale appropriately to traffic. Elastic beanstalk will use a **Load Balancer** to direct the requests to different EC2 instances.
2. **EC2 Instance Type** to **t3.micro** because it is within the free tier (for 12 months) and is sufficiently powerful to support our web API.

Then click the **Publish** button to publish onto Elastic Beanstalk.
![Publish to AWS Settings](Diagrams/screenshots/Beanstalk/Publish_To_AWS_Settings.png)

Then it would take a few minutes to publish the web API onto Elastic Beanstalk. Eventually the process will finish.

![Published to Elastic Beanstalk](Diagrams/screenshots/Beanstalk/Beanstalk_Published.png)

Click on the endpoint will open up the elastic beanstalk hosted website.
![Elastic Beanstalk Endpoint Home](Diagrams/screenshots/Beanstalk/Endpoint_Home.png)

All seems to be working alright, but actually if we go to the `/api/Airport` endpoint (on browser, so it would be `GET` request), it actually doesn't load (`500 internal server error`)
![Elastic Beanstalk API Airport Error](Diagrams/screenshots/Beanstalk/Endpoint_API_Airport_Error.png)

This is because the EC2 instances of Elastic Beanstalk don't have the correct permission to access the DynamoDB **airports** table.

We can grant them the permission to access the table by go to the **IAM service** > **Roles**
![IAM Service Roles](Diagrams/screenshots/Beanstalk/IAM_Roles.png)

We can see one of the latest roles has **Role name** of `AirportAPI-....` and has **Trusted entities** of `AWS Service: ec2`. This is the role used by the EC2 instances of the Elastic Beanstalk that hosts our endpoints.
![IAM Service Roles EC2](Diagrams/screenshots/Beanstalk/IAM_Roles_EC2.png)

Click it and we see this role only has Permissions Policies relating to Elastic Beanstalk, this is why it doesn't have access to DynamoDB tables.
We can grant it permission by clicking **Add permissions** > **Attach policies**
![IAM Service Roles EC2 Permissions](Diagrams/screenshots/Beanstalk/IAM_Roles_EC2_Permissions_Policies.png)

Then search for policies **dynamodb** and tick the **AmazonDynamoDBFullAccess** policy, and click the bottom right button **Attach policies**
![IAM Service Roles EC2 Permissions Attach](Diagrams/screenshots/Beanstalk/IAM_Roles_EC2_Attach.png)

Now that the new DynamoDB access policy is attached to the EC2 instances of Elastic Beanstalk, wait a few seconds and try the `/api/Airport` again, and we should see that it works.
![Elastic Beanstalk API Airport Success](Diagrams/screenshots/Beanstalk/Endpoint_API_Airport_Success.png)

You can also go to `/swagger` endpoint to easily test out all the endpoints, they should all work as expected. 🎉

</details>

<details>
<summary><span style="font-size:1.2rem">Delete Elastic Beanstalk Instance</span></summary>

Now that we've successfully published our local Web API onto Elastic Beanstalk, it is time to turn it off, shut it down, before we forget about it 12 months later and start getting charged for those EC2 instances.

To delete the elastic beanstalk, go to AWS Explorer > AWS Elastic Beanstalk > AirportAPI, right click > **Delete**

![Delete Elastic Beanstalk](Diagrams/screenshots/Beanstalk/Delete_Elastic_Beanstalk.png)

This deletion process would take a few minutes, eventualy we can check on the AWS website > Elastic Beanstalk and see the application for `AirportAPI-dev` deleted.

![Deleted Successful](Diagrams/screenshots/Beanstalk/Deleted_Successful.png)

Feel free to also do the same deletion to the DynamoDB **airports** table, and the IAM roles for Elastic Beanstalk.
