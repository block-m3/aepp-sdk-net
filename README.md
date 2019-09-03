# aepp-sdk-net
.NET SDK to interact with the Æternity blockchain

Master is beta, not for production enviroment.

[![Build status](https://ci.appveyor.com/api/projects/status/7xeyr1qx2opxy5jn/branch/master?svg=true)](https://ci.appveyor.com/project/maxpiva/aepp-sdk-net/branch/master)

Loosely based on the following sdks:

* [aepp-sdk-java](https://github.com/kryptokrauts/aepp-sdk-java)
* [aepp-sdk-python](https://github.com/aeternity/aepp-sdk-python)
* [aepp-sdk-js](https://github.com/aeternity/aepp-sdk-js)

Supports

AEternity >= v4.2.0 & Sophia >= v3.2.0


* All the code is async. But Sync extension methods exists for easy use.
* Net Standard 2.0. Both .NET Core and .NET Framework are supported
* You can use the FlatClient for porting (it maps all the Node/Compiler functions), or the normal client which is fluent and easy to use.
* OracleService and AsyncOracleService provides a background service capable of running an oracle.
* Clients Configuration constructor can use IConfiguration and ILogger for dependency injection on ASP NET projects.
* OracleService/AsyncOracleService are based on BackgroundService, which can also be used on ASP NET projects as a singleton in the startup.
* You can subclass the Configuration Class and override GetHttpClient so you can provide a custom HttpClient Factory if needed.
* The Fluent Client serialize/de-serialize inputs and outputs into .net objects, so you don't have to worry about serializing or de-serializing stuff from contracts or oracles.
* Several other features, like automanaging of the nonce, Easy checking or waiting for transaction finish, combination functions like MeasureAndCall (Dry & Call), etc.

# Examples

## Transfer

```csharp
Configuration cfg = new Configuration(); // You can inject here IConfiguration and ILogger classes.
cfg.NativeMode = true;
cfg.Network = Constants.Network.DEVNET;
Client client = new Client(cf);
Account from = client.ConstructAccount(new BaseKeyPair("79816BBF860B95600DDFABF9D81FEE81BDB30BE823B17D80B9E48BE0A7015ADF"));
BaseKeyPair tokp = BaseKeyPair.Generate();
Account to = client.ConstructAccount(kp);
ulong transfer = (ulong)1m.ToAettos(Unit.AE);
account.SendAmount(to.KeyPair.PublicKey, transfer).WaitForFinish(TimeSpan.FromSeconds(30));
to.Refresh();
Assert.AreEqual(to.Balance,transfer);
```

## Name Service

```csharp
Configuration cfg = new Configuration(); // You can inject here IConfiguration and ILogger classes.
cfg.NativeMode = true;
cfg.Network = Constants.Network.DEVNET;
Client client = new Client(cf);
Random random = new Random();
string domain = "aeternity" + random.Next() + ".test";
Account account = client.ConstructAccount(new BaseKeyPair("79816BBF860B95600DDFABF9D81FEE81BDB30BE823B17D80B9E48BE0A7015ADF"));
PreClaim preclaim = account.PreClaimDomain(domain).WaitForFinish(TimeSpan.FromSeconds(30));
Claim claim = preclaim.ClaimDomain().WaitForFinish(TimeSpan.FromSeconds(30));
claim = claim.Update(10000, 50).WaitForFinish(TimeSpan.FromSeconds(30));
claim.Revoke().WaitForFinish(TimeSpan.FromSeconds(30));
```

## Contracts

```csharp
Configuration cfg = new Configuration(); // You can inject here IConfiguration and ILogger classes.
cfg.NativeMode = true;
cfg.Network = Constants.Network.DEVNET;
Client client = new Client(cf);
Account account = client.ConstructAccount(new BaseKeyPair("79816BBF860B95600DDFABF9D81FEE81BDB30BE823B17D80B9E48BE0A7015ADF"));
Contract contract = account.ConstructContract(IdentityContractSourceCode); //string containing the contract source
contract.Deploy(0, 0, 2000000000, 100000).WaitForFinish(TimeSpan.FromSeconds(30));
ContractReturn<int> re = contract.StaticCall<int>("main", 0, 42);
Assert.AreEqual(re.ReturnValue, 42);
```

```csharp
Configuration cfg = new Configuration(); // You can inject here IConfiguration and ILogger classes.
cfg.NativeMode = true;
cfg.Network = Constants.Network.DEVNET;
Client client = new Client(cf);
Account account = client.ConstructAccount(new BaseKeyPair("79816BBF860B95600DDFABF9D81FEE81BDB30BE823B17D80B9E48BE0A7015ADF"));
BaseKeyPair rec1 = BaseKeyPair.Generate();
BaseKeyPair rec2 = BaseKeyPair.Generate();
BaseKeyPair rec3 = BaseKeyPair.Generate();
//map(address, int)
Dictionary<string, int> input = new Dictionary<string, int>();
input.Add(rec1.PublicKey, 40);
input.Add(rec2.PublicKey, 40);
input.Add(rec3.PublicKey, 20);
ulong paymentValue = (ulong)1m.ToAettos(Unit.AE);
Contract contract = account.ConstructContract(paymentSplitterSource); //string containing the contract source
contract.MeasureAndDeploy(0, 0, Constants.BaseConstants.MINIMAL_GAS_PRICE, "init", input).WaitForFinish(TimeSpan.FromSeconds(30));
contract.MeasureAndCall("payAndSplit", Constants.BaseConstants.MINIMAL_GAS_PRICE, paymentValue).WaitForFinish(TimeSpan.FromSeconds(30));
```

## Oracles

```csharp

public class CityQuery
{
    [SophiaName("city")]
    public string City { get; set; }
}

public class TemperatureResponse
{
    [SophiaName("temp_c")]
    public int TemperatureCelsius { get; set; }
}

public class CityTemperatureService : OracleService<CityQuery, TemperatureResponse>
{
    public override TemperatureResponse Answer(CityQuery ask)
    {
        if (ask.City.ToLowerInvariant() == "montevideo")
            return new TemperatureResponse {TemperatureCelsius = 24};
        if (ask.City.ToLowerInvariant() == "sofia")
            return new TemperatureResponse {TemperatureCelsius = 25};
        return new TemperatureResponse {TemperatureCelsius = 2000};
    }
}




Configuration cfg = new Configuration(); // You can inject here IConfiguration and ILogger classes.
cfg.NativeMode = true;
cfg.Network = Constants.Network.DEVNET;
Client client = new Client(cf);
Account from = client.ConstructAccount(new BaseKeyPair("79816BBF860B95600DDFABF9D81FEE81BDB30BE823B17D80B9E48BE0A7015ADF"));
BaseKeyPair tokp = BaseKeyPair.Generate();
Account oracleaccount = client.ConstructAccount(kp);
ulong transfer = (ulong)1m.ToAettos(Unit.AE);
account.SendAmount(oracleaccount.KeyPair.PublicKey, transfer).WaitForFinish(TimeSpan.FromSeconds(30)); //Send some money to the oracle
oracleAccount.Refresh();

//Server part
OracleQuery<CityQuery, TemperatureResponse> query = oracleAccount.RegisterOracle<CityQuery, TemperatureResponse>().WaitForFinish(TimeSpan.FromSeconds(30));
CityTemperatureService svc = new CityTemperatureService();
svc.Query = query; //Assign the query to the Service
Task.Factory.StartNew(svc.Start); //Starts the CityTemperature Service, server will run on other thread.

//Client Part
RegisteredOracle<CityQuery, TemperatureResponse> reg = account.GetOracle<CityQuery, TemperatureResponse>(query.Id);
TemperatureResponse resp = reg.Ask(new CityQuery {City = "montevideo"}).WaitForFinish(TimeSpan.FromSeconds(30));
Assert.AreEqual(resp.TemperatureCelsius, 24);
resp = reg.Ask(new CityQuery {City = "sofia"}).WaitForFinish(TimeSpan.FromSeconds(30));
Assert.AreEqual(resp.TemperatureCelsius, 25);
resp = reg.Ask(new CityQuery {City = "hell"}).WaitForFinish(TimeSpan.FromSeconds(30));
Assert.AreEqual(resp.TemperatureCelsius, 2000);
svc.Stop(); //Stop the service
```


**Integration Testing on Windows 10 and Visual Studio How-To**

1) Install Docker For Windows https://docs.docker.com/docker-for-windows/install

2) Make sure port 80 is not binded in your computer (IIS, Apache, etc).

3) CD to the root of this project.

4) type: *docker-compose up*

**TODO**

- State Channels are wip, not usable yet. websocket code is not started yet.
- Serialization/Deserialization code for Contracts inputs and outputs are wacky. Requires better code,understanding of the contract input/output formats and testing. 
- Better error handling.
- nugets will be released, when this library jump out of beta.
