namespace IndieBot101.Dialogs

{

    using Microsoft.Bot.Builder.Dialogs;

    using System;

    using System.Threading.Tasks;

    using Microsoft.Bot.Connector;



    [Serializable]

    public class CityDialog : IDialog<string>

    {

        private string name;

        private int attempts = 3;



        public CityDialog(string name)

        {

            this.name = name;

        }



        public async Task StartAsync(IDialogContext context)

        {

            await context.PostAsync($" Which city are you located in, {this.name} ?");



            context.Wait(this.MessageReceivedAsync);

        }



        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)

        {

            var message = await result;



            if ((message.Text != null) && (message.Text.Trim().Length > 0))

            {

                /* Completes the dialog, removes it from the dialog stack, and returns the result to the parent/calling

                    dialog. */

                context.Done(message.Text);

            }

            else

            {

                --attempts;

                if (attempts > 0)

                {

                    await context.PostAsync("I'm sorry, I don't understand your reply. Which city are you in?");



                    context.Wait(this.MessageReceivedAsync);

                }

                else

                {

                    context.Fail(new TooManyAttemptsException("Message was not a valid City."));

                }

            }

        }

    }

}