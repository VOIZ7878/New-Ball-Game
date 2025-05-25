using System;
using System.Threading.Tasks;
using BallGame.Input;
using System.Windows.Forms;

namespace BallGame
{
    public class WinFormsMenuManager : IMenuManager
    {
        private readonly Form form;
        private TaskCompletionSource<MenuChoice>? menuTcs;

        public WinFormsMenuManager(Form form)
        {
            this.form = form;
        }

        public Task<MenuChoice> ShowMainMenuAsync()
        {
            menuTcs = new TaskCompletionSource<MenuChoice>();
            return menuTcs.Task;
        }

        public void SelectOption(MenuChoice choice)
        {
            if (menuTcs != null && !menuTcs.Task.IsCompleted)
            {
                menuTcs.SetResult(choice);
            }
        }
    }
}