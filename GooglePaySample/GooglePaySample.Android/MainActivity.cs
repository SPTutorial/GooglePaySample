using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using Xamarin.Forms;

namespace GooglePaySample.Droid
{
    [Activity(Label = "GooglePaySample", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        int gpay_requestCode = 1023;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
            MessagingCenter.Subscribe<string>(this, "PayViaGooglePay", (value) =>
              {
                  PayViaGooglePay();
              });
        }
        private void PayViaGooglePay()
        {
            if (IsGooglePayInstalled())
            {
                string transId = $"UPI{Guid.NewGuid().ToString().Substring(0, 8)}";
                using (var uri = new Android.Net.Uri.Builder()
                    .Scheme("upi")
                    .Authority("pay")
                    .AppendQueryParameter("pa", "yourgooglepayudername@test")
                    .AppendQueryParameter("pn", "Test Name")
                    .AppendQueryParameter("pn", "Seding Amount of RS 1")
                    .AppendQueryParameter("mc", "0000")
                    .AppendQueryParameter("tr", transId)
                    .AppendQueryParameter("tn", "Pay to Test Name")
                    .AppendQueryParameter("am", "1")
                    .AppendQueryParameter("cu", "INR")
                    .Build())
                {
                    Intent = new Intent(Intent.ActionView);
                    Intent.SetData(uri);
                    Intent.SetPackage("com.google.android.apps.nbu.paisa.user");
                    StartActivityForResult(Intent, gpay_requestCode);
                }
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == gpay_requestCode)
            {
                if (data != null)
                {
                    string response = data.GetStringExtra("response");
                    string[] resArray = response.Split("&");

                    string txnId = resArray[0].Split("=")[1].ToString();

                    string responseCode = resArray[1].Split("=")[1].ToString();

                    string status = resArray[2].Split("=")[1].ToString();

                    string txnRef = resArray[3].Split("=")[1].ToString();

                    if(status =="SUCCESS")
                    {
                        Toast.MakeText(this, "Payment Success", ToastLength.Long).Show();
                    }
                    else
                    {
                        Toast.MakeText(this, "Payment Failed", ToastLength.Long).Show();
                    }
                }
                else
                {
                    Toast.MakeText(this, "Payment Failed", ToastLength.Long).Show();
                }
            }
        }

        private bool IsGooglePayInstalled()
        {
            PackageManager pm = this.PackageManager;
            bool installed = false;
            try
            {
                pm.GetPackageInfo("com.google.android.apps.nbu.paisa.user", PackageInfoFlags.Activities);
                installed = true;
            }
            catch (PackageManager.NameNotFoundException e)
            {
                Toast.MakeText(this, "Google Pay Is Not Installed", ToastLength.Long).Show();
            }
            return installed;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}