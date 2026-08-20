﻿#if MONETIZATION_PURCHASING_ENABLE_IAP_V4
using System;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Purchasing;
using WarehouseKeeper._WarehouseKeeper.Scripts.Shops.Monetization.Purchasing.IAP.UnityServices;

#pragma warning disable 618

namespace WarehouseKeeper._WarehouseKeeper.Scripts.Shops.Monetization.Purchasing.IAP.v4
{
internal sealed class UnityIAPLoggerV4
{
    private int _initializeSdkAsync;
    private int _onInitialized;

    private int _initializedFailedMini;
    private InitializationFailureReason _initializedFailedMiniError;

    private int _initializedFailedMega;
    private InitializationFailureReason _initializedFailedMegaError;
    private string _initializedFailedMegaMessage = string.Empty;

    private int _requestToPurchase;
    private string _requestToPurchaseLastId;

    private int _confirmPurchase;
    private string _confirmPurchaseLastId;

    private int _processPurchase;
    private string _processPurchaseLastId;

    private int _purchaseFailedMini;
    private string _purchaseFailedMiniLastId;
    private PurchaseFailureReason _purchaseFailedMiniLastReason;

    private int _purchaseFailedMega;
    private string _purchaseFailedMegaLastId;
    private PurchaseFailureReason _purchaseFailedMegaLastReason;
    private string _purchaseFailedMegaMessage = string.Empty;

    #region Initialization

    public void LogInitializeSdkAsync() => _initializeSdkAsync++;

    public void LogOnInitialized() => _onInitialized++;

    public void LogOnInitializeFailed(InitializationFailureReason error)
    {
        _initializedFailedMini++;
        _initializedFailedMiniError = error;
        Debug.LogError($"[{nameof(UnityIAPProviderV4)}] Initialize Failed: {error}");
    }

    public void LogOnInitializeFailed(InitializationFailureReason error, string message)
    {
        _initializedFailedMega++;
        _initializedFailedMegaError = error;
        _initializedFailedMegaMessage = message;
        Debug.LogError($"[{nameof(UnityIAPProviderV4)}] Initialize Failed: error={error}; message={message}");
    }

    #endregion

    #region Purchasing

    public void LogRequestToPurchase(string productId)
    {
        _requestToPurchase++;
        _requestToPurchaseLastId = productId;
    }

    public void LogConfirmPurchase(Product product)
    {
        _confirmPurchase++;
        _confirmPurchaseLastId = product.definition.id;
    }

    public void LogProcessPurchase(Product product)
    {
        _processPurchase++;
        _processPurchaseLastId = product.definition.id;
        Debug.Log("transaction: " + product.transactionID);
        Debug.Log(GetPurchaseInfo(product));
    }

    public void LogOnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        _purchaseFailedMini++;
        _purchaseFailedMiniLastId = product.definition.id;
        _purchaseFailedMiniLastReason = failureReason;
        Debug.LogError(
            $"[{nameof(UnityIAPProviderV4)}] Purchase failed: {product.definition.id}, Reason: {failureReason}");
    }

    public void LogOnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        _purchaseFailedMega++;
        _purchaseFailedMegaLastId = product.definition.id;
        _purchaseFailedMegaLastReason = failureDescription.reason;
        _purchaseFailedMegaMessage = failureDescription.message;
        Debug.LogError($"Purchase failed: {product.definition.id}, " +
                       $"Reason: {failureDescription.reason}, " +
                       $"Message: {failureDescription.message}");
    }

    #endregion

    public string GetDebugIAPStatus(IStoreController controller,
                                    IExtensionProvider extension,
                                    IAPConfigurationCollection products)
    {
        var sb = new StringBuilder();

        #region General

        sb.Append("ISA=").Append(_initializeSdkAsync).Append(';');            // InitializeSdkAsync
        sb.Append("Crl=").Append(controller != null ? 1 : 0).Append(';');     // Controller exists
        sb.Append("Ext=").Append(extension != null ? 1 : 0).Append(';');      // Extension Exist
        sb.Append("Pr=").Append(products.products?.Length ?? -1).Append(';'); // Count Products init
        sb.Append("IsGP=").Append(IsInstallFromGooglePlay()).Append(';');     // Verify Install Source
        sb.Append("USM=").Append(UnityServicesManager.isInitialize ? 1 : 0)   // Unity Services Manager
          .Append('|')
          .Append(string.IsNullOrEmpty(UnityServicesManager.lastError) ? '-' : UnityServicesManager.lastError)
          .Append(';');

        #endregion

        #region Initialize

        sb.Append("oI=").Append(_onInitialized).Append(';'); // OnInitialized
        sb.Append("oIFMin=").Append(_initializedFailedMini)  // OnInitializeFailed Mini
          .Append('|')
          .Append(_initializedFailedMini > 0 ? _initializedFailedMiniError : '-')
          .Append(';');

        sb.Append("oIFMeg=").Append(_initializedFailedMega) // OnInitializeFailed Mega
          .Append('|')
          .Append(_initializedFailedMega > 0 ? _initializedFailedMegaError : '-')
          .Append('|')
          .Append(_initializedFailedMega > 0 ? _initializedFailedMegaMessage : '-')
          .Append(';');

        #endregion

        #region Purchasing

        sb.Append("RTP=").Append(_requestToPurchase) // RequestToPurchase
          .Append('|')
          .Append(_requestToPurchase > 0 ? _requestToPurchaseLastId : '-')
          .Append(';');

        sb.Append("CP=").Append(_confirmPurchase) // ConfirmPurchase
          .Append('|')
          .Append(_confirmPurchase > 0 ? _confirmPurchaseLastId : '-')
          .Append(';');

        sb.Append("PP=").Append(_processPurchase) // ProcessPurchase
          .Append('|')
          .Append(_processPurchase > 0 ? _processPurchaseLastId : '-')
          .Append(';');

        sb.Append("oPFMin=").Append(_purchaseFailedMini) // LogOnPurchaseFailed Mini
          .Append('|')
          .Append(_purchaseFailedMini > 0 ? _purchaseFailedMiniLastId : '-')
          .Append('|')
          .Append(_purchaseFailedMini > 0 ? _purchaseFailedMiniLastReason : '-')
          .Append(';');

        sb.Append("oPFMeg=").Append(_purchaseFailedMega) // LogOnPurchaseFailed Mega
          .Append('|')
          .Append(_purchaseFailedMega > 0 ? _purchaseFailedMegaLastId : '-')
          .Append('|')
          .Append(_purchaseFailedMega > 0 ? _purchaseFailedMegaLastReason : '-')
          .Append('|')
          .Append(_purchaseFailedMega > 0 ? _purchaseFailedMegaMessage : '-')
          .Append(';');

        #endregion

        return sb.ToString();
    }

    private static bool IsInstallFromGooglePlay()
    {
#if UNITY_ANDROID
        try
        {
            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            using var packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager");

            var packageName = currentActivity.Call<string>("getPackageName");
            var googlePlayPackage = packageManager.Call<string>("getInstallerPackageName", packageName);

            return googlePlayPackage == "com.android.vending";
        }
        catch (Exception)
        {
            Debug.LogError($"[{nameof(UnityIAPLoggerV4)}] Error verify installer source");

            return false;
        }
#else
        return false;
#endif
    }

    public string GetPurchaseInfo(Product product)
    {
        if (product == null) return " PurchaseEventArgs is null.";

        var result = $" PurchaseEventArgs:\n";
        result += $"- Product ID: {product.definition.id}\n";
        result += $"- Store Specific ID: {product.definition.storeSpecificId}\n";
        result += $"- Product Type: {product.definition.type}\n";

        var meta = product.metadata;
        result += "\n Metadata:\n";
        result += $"- Title: {meta.localizedTitle}\n";
        result += $"- Description: {meta.localizedDescription}\n";
        result += $"- Price: {meta.localizedPrice} {meta.isoCurrencyCode}\n";
        result += $"- Price String: {meta.localizedPriceString}\n";
        result += $"- Transaction ID: {product.transactionID}\n";

        result += "\nRaw Receipt:\n";
        if (string.IsNullOrEmpty(product.receipt))
        {
            result += "- Receipt is null or empty.\n";

            return result;
        }

        result += "- Full Receipt JSON:\n";
        result += product.receipt + "\n";

        try
        {
            JObject root = JObject.Parse(product.receipt);

            result += "\n Parsed Receipt Info:\n";
            string store = root["Store"]?.ToString();
            string transactionID = root["TransactionID"]?.ToString();

            result += $"- Store: {store}\n";
            result += $"- Transaction ID: {transactionID}\n";

#if UNITY_ANDROID
            string payload = root["Payload"]?.ToString();
            JObject googlePayload = JObject.Parse(payload);
            result += "- Google JSON Fields:\n";
            result += $"  • json: {googlePayload["json"]}\n";
            result += $"  • signature: {googlePayload["signature"]}\n";
#elif UNITY_IOS
            result += "- iOS Base64 Receipt:\n";
            result += payload.Length > 300
                ? payload.Substring(0, 300) + "..."
                : payload;
#endif
        }
        catch (Exception ex)
        {
            result += $"\nError parsing receipt JSON: {ex.Message}\n";
        }

        return result;
    }
}
#endif
