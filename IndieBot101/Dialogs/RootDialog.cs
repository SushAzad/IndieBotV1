namespace IndieBot101.Dialogs

{

    using System;

    using System.Net;

    using System.Threading.Tasks;

    using Microsoft.Bot.Builder.Dialogs;

    using Microsoft.Bot.Connector;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;

    [Serializable]
    public class supplier
    {
        public string name { get; set; }
        public string address { get; set; }
        public string phone
        {
            get; set;

        }
    }

#pragma warning disable 1998

        [Serializable]



        public class RootDialog : IDialog<object>

        {



            private string name;
            private int option;

            private string grain;

            private string city;
            private string forecast;





            public async Task StartAsync(IDialogContext context)

            {

                /* Wait until the first message is received from the conversation and call MessageReceviedAsync 

                 *  to process that message. */

                context.Wait(this.MessageReceivedAsync);

            }



            private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)

            {

                /* When MessageReceivedAsync is called, it's passed an IAwaitable<IMessageActivity>. To get the message,

                 *  await the result. */

                var message = await result;



                await this.SendWelcomeMessageAsync(context);

            }
        private async Task GoodbyeMessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)

        {

            /* When MessageReceivedAsync is called, it's passed an IAwaitable<IMessageActivity>. To get the message,

             *  await the result. */

            var message = await result;


            context.Done(message);

        }



        private async Task SendWelcomeMessageAsync(IDialogContext context)

            {

                await context.PostAsync("Hi, I'm Grameen Saathi. Currently I can help you find agricultural product dealers near you, or get a weather forecast. Let's get started.");



                context.Call(new NameDialog(), this.NameDialogResumeAfter);

            }
            private async Task SendGoodbyeMessageAsync(IDialogContext context)

            {

                await context.PostAsync($"Thank you for using Grameen Saathi {name}. I hope your question was answered correctly!");


            context.Wait(this.GoodbyeMessageReceivedAsync);
            
            //context.Call(new OptionDialog(), this.OptionDialogResumeAfter);
                

            }


            private async Task NameDialogResumeAfter(IDialogContext context, IAwaitable<string> result)

            {

                try

                {

                    this.name = await result;



                    context.Call(new OptionDialog(), this.OptionDialogResumeAfter);

                }

                catch (TooManyAttemptsException)

                {

                    await context.PostAsync($"I'm sorry, I'm having issues understanding you. Let's try again.");



                    await this.SendWelcomeMessageAsync(context);

                }

            }
            private async Task OptionDialogResumeAfter(IDialogContext context, IAwaitable<string> result)

            {

                try

                {
                    string choiceResponse = await result;
                    if (choiceResponse.ToLower().Contains("agriculture") || choiceResponse.ToLower().Contains("grain") || choiceResponse.ToLower().Contains("dealer") || choiceResponse.ToLower().Contains("purchase") || choiceResponse.ToLower().Contains("sell") || choiceResponse.ToLower().Contains("crops") || choiceResponse.ToLower().Contains("buy") || choiceResponse.ToLower().Contains("product"))
                {
                        this.option = 1;
                    }
                    else
                    {
                        this.option = 2;
                    }


                    context.Call(new CityDialog(this.name), this.CityDialogResumeAfter);

                }

                catch (TooManyAttemptsException)

                {

                    await context.PostAsync($"I'm sorry, I'm having issues understanding you. Let's try again.");



                    await this.SendWelcomeMessageAsync(context);

                }

            }

            private async Task CityDialogResumeAfter(IDialogContext context, IAwaitable<string> result)

            {

                try

                {
                    this.city = await result;
                    if (this.option == 1)
                    {
                        context.Call(new GrainDialog(this.name), this.GrainDialogResumeAfter);
                    }

                    else
                    {
                        context.Call(new WeatherDialog(this.city), this.WeatherDialogResumeAfter);
                    }

                }

                catch (TooManyAttemptsException)
                {
                    await context.PostAsync($"I'm sorry, I'm having issues understanding you. Let's try again.");
                    await this.SendWelcomeMessageAsync(context);
                }

            }
            private async Task WeatherDialogResumeAfter(IDialogContext context, IAwaitable<string> result)

            {

                try

                {

                    this.forecast = await result;



                    await context.PostAsync($"{forecast}");
                await this.SendGoodbyeMessageAsync(context);


                }

                catch (TooManyAttemptsException)

                {

                    await context.PostAsync($"I'm sorry, I'm having issues understanding you. Let's try again.");



                    await this.SendWelcomeMessageAsync(context);

                }

            }

            private async Task GrainDialogResumeAfter(IDialogContext context, IAwaitable<string> result)

            {

                try

                {
                    List<supplier> suppliers = new List<supplier>();

                    this.grain = await result;
                    await context.PostAsync("Hold on, let me fetch that for you.");
                    var url = "https://dir.indiamart.com/search.mp?ss=" + grain + "&cq=" + city;
                    HttpWebRequest request = WebRequest.Create($"{ url }") as HttpWebRequest;
                    //optional
                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                    string respon = readStream.ReadToEnd();
                    System.IO.File.WriteAllText(@"C:\Users\suazad\Documents\Hackathon2017\WriteText1.html", respon);
                    HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                    htmlDoc.Load(@"C:\Users\suazad\Documents\Hackathon2017\WriteText1.html");

                    if (htmlDoc.DocumentNode != null)
                    {

                        int resultCount = 0;
                        foreach (HtmlAgilityPack.HtmlNode node in htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'lst')]"))
                        {

                            //await context.PostAsync($"{resultCount} ");
                            if (resultCount > 6)
                                break;
                            foreach (HtmlAgilityPack.HtmlNode node1 in node.SelectNodes("//div[@class='nes']"))
                            {
                                supplier sup = new supplier();
                                string supplier = "";
                                // string website = "";
                                if (resultCount > 6)
                                    break;
                                foreach (HtmlAgilityPack.HtmlNode node2 in node1.SelectNodes(".//a"))
                                {
                                    supplier = node2.InnerText;
                                    sup.name = supplier;
                                    //    website = node2.Attributes["href"].Value;
                                }
                                foreach (HtmlAgilityPack.HtmlNode node2 in node1.SelectNodes(".//span[@class='srad cty-t']"))
                                {
                                    string address = node2.InnerText;
                                    //await context.PostAsync($"{resultCount}){ supplier }, Address: {address} ");
                                    sup.address = address;


                                    //await context.PostAsync($"{ address }");
                                }
                                suppliers.Add(sup);
                                resultCount++;
                            }
                            int phoneCount = 0;
                            foreach (HtmlAgilityPack.HtmlNode node1 in node.SelectNodes("//span[@class='ls_co phn']"))
                            {
                                if (phoneCount > 6)
                                    break;
                                string phoneNumber = node1.InnerText;
                                //phoneNumber = phoneNumber.Substring(4);
                                suppliers[phoneCount].phone = phoneNumber.Substring(4, 10);
                                phoneCount++;

                            }

                            //resultCount++;
                        }
                    }
                    int supCount = 1;
                    foreach (supplier suply in suppliers)
                    {
                        await context.PostAsync($"{supCount}) {suply.name} Address:  {suply.address} Contact: {suply.phone}");
                        supCount++;
                    }

                }

                catch (TooManyAttemptsException)

                {

                    await context.PostAsync("I'm sorry, I'm having issues understanding you. Let's try again.");

                }

                finally

                {

                    await this.SendGoodbyeMessageAsync(context);

                }

            }

        }

    }
