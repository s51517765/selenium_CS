using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using OpenQA.Selenium.Chrome;
using System.Threading;
using OpenQA.Selenium;

namespace AmazonTwitter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        ChromeDriver driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));


        private void Form1_Load(object sender, EventArgs e)
        {
            //https://gazee.net/develop/csharp-selenium-chrome-driver/
         
            // textBox multiLine はwardWrap =falseにする
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            login();
            //  driver.Url = "http://amazon.co.jp/";
            Thread.Sleep(6000);

            string[] keyword = new string[100];
            keyword = textBoxInputKeyword.Text.Split(); //改行区切りで配列に入れる
            string resultNo = "result_0"; //検索結果の1番目

            for (int i = 0; i < keyword.Length; i++)
            {
                if (keyword[i] != "")
                {
                    driver.SwitchTo().Window(driver.WindowHandles.First());
                    string curentUrl = driver.Url;
                    driver.FindElementById("twotabsearchtextbox").Clear();
                    driver.FindElementById("twotabsearchtextbox").SendKeys(keyword[i]);
                    Thread.Sleep(1000);
                    driver.FindElementById("twotabsearchtextbox").SendKeys(OpenQA.Selenium.Keys.Return);                   
                    Thread.Sleep(1000);

                    while (driver.FindElement(By.Id(resultNo)) == null) ; // using OpenQA.Selenium;
                    Thread.Sleep(1000);
                    driver.FindElementById(resultNo).Click();
                    driver.SwitchTo().Window(driver.WindowHandles.Last());
                    twitter();
                }
            }
        }
        private void login()
        {
            var auth = new auth();  //別のクラスファイルにメールアドレスとパスワードを記録
            string email = auth.email;
            string password = auth.password;

            //ログイン
            string url = "https://www.amazon.co.jp/ap/signin?_encoding=UTF8&ignoreAuthState=1&openid.assoc_handle=jpflex&openid.claimed_id=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select&openid.identity=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select&openid.mode=checkid_setup&openid.ns=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0&openid.ns.pape=http%3A%2F%2Fspecs.openid.net%2Fextensions%2Fpape%2F1.0&openid.pape.max_auth_age=0&openid.return_to=https%3A%2F%2Fwww.amazon.co.jp%2Fgp%2Fyourstore%2Fhome%3Fie%3DUTF8%26ref_%3Drhf_custrec_signin&switch_account=";
            driver.Url = url;
            Thread.Sleep(1000);

            driver.FindElementById("ap_email").SendKeys(email);
            driver.FindElementById("ap_password").SendKeys(password);
            driver.FindElementById("signInSubmit").Click();
        }
        private void twitter()
        {
            var auth = new auth();
            string twitter = auth.twitter;
            string tw_password = auth.tw_password;
            Thread.Sleep(3000);
            driver.FindElementByXPath("//*[@id='amzn-ss-twitter-share']/div/a/i").Click();
            Thread.Sleep(1000);
            driver.SwitchTo().Window(driver.WindowHandles.Last());
            string text = driver.FindElementById("status").Text;
            text = text.Replace("@さんから", "");
            text = text.Replace("を Amazon でチェック！", "");
            textBoxLog.AppendText(text + "\n");

            try
            {
                try
                {   //Loginしてないとき
                    driver.FindElementById("username_or_email").SendKeys(twitter);
                    driver.FindElementById("password").SendKeys(tw_password);
                    driver.FindElementByXPath("/html/body/div[2]/form/div[3]/fieldset[2]/input").Click();
                }
                catch
                {
                    //Loginずみ
                    driver.FindElementByXPath("/html/body/div[2]/form/div[3]/fieldset/input").Click(); //XPathはfirefoxで調べる
                }
            }
            catch
            {
                textBoxLog.AppendText("Tweet Error!" + "\n");
                driver.Close(); //タブを閉じる
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Fromが閉じられるときdriverを閉じる
            driver.Quit();
        }
    }

}
