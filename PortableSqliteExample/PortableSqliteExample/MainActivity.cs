using Android.App;
using Android.Widget;
using Android.OS;
using System.Linq;
using System.Collections.Generic;
using Android.Runtime;
using System;
using Android.Support.Design.Widget;
using static Android.Views.View;
using Android.Views;
using static Android.Widget.AdapterView;
using Android.Support.V7.App;
using Android.Webkit;
using Android.Views.InputMethods;
using Android.Content;
using static Android.Widget.TextView;
using Android.Support.V4.Content;

namespace PortableSqliteExample
{
    [Activity(Label = "PortableSqliteExample", MainLauncher = true, Icon = "@drawable/icon")]//, Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class MainActivity : AppCompatActivity, IOnItemClickListener, IOnEditorActionListener
    {
        private int _selectedId { get; set; }
        private Android.Support.V7.Widget.Toolbar _Toolbar { get; set; }
        private IMenuItem _SearchAction;
        private bool isSearchOpened = false;
        private EditText edtSeach;
        private IBinder _Token { get; set; }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            Button btnInsert = FindViewById<Button>(Resource.Id.btnInsert);
            btnInsert.Click += delegate { OnInsertClick(); };
            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Visibility = Android.Views.ViewStates.Gone;
            fab.Click += delegate { DeleteItem(); };

            _Toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(_Toolbar);

            //Acessing AppStartup to check if there's already stored values.
            var users = AppStartup.Current.Manager.UserRepository.Get();

            if (users.Any())
            {
                FillListView(users.ToList());
            }
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu, menu);

            _SearchAction = menu.FindItem(Resource.Id.action_search);
            //Resource.
            return base.OnPrepareOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            switch (id)
            {
                //case Resource.Id.action_settings:
                //    return true;
                case Resource.Id.action_search:
                    HandleMenuSearch();
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void HandleMenuSearch()
        {
            Android.Support.V7.App.ActionBar action = SupportActionBar; //get the actionbar

            var imm = (InputMethodManager)GetSystemService(Context.InputMethodService);

            //edtSeach = (EditText)action.CustomView.FindViewById(Resource.Id.edtSearch);

            if (isSearchOpened)
            { //test if the search is open
                DisableSearch();
            }
            else
            { //open the search entry

                action.SetDisplayShowCustomEnabled(true); //enable it to display a
                                                          // custom view in the action bar.
                action.SetCustomView(Resource.Layout.SearchBar);//add the custom view
                action.SetDisplayShowTitleEnabled(false); //hide the title

                edtSeach = (EditText)action.CustomView.FindViewById(Resource.Id.edtSearch); //the text editor

                //this is a listener to do a search when the user clicks on search button
                edtSeach.SetOnEditorActionListener(this);


                edtSeach.RequestFocus();

                //open the keyboard focused in the edtSearch
                //InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
                imm.ShowSoftInput(edtSeach, ShowFlags.Implicit);

                _Token = edtSeach.WindowToken;

                //add the close icon
                _SearchAction.SetIcon(ContextCompat.GetDrawable(this, Resource.Drawable.Forward48));

                isSearchOpened = true;
            }
        }

        private void DisableSearch()
        {
            SupportActionBar.SetDisplayShowCustomEnabled(false); //disable a custom view inside the actionbar
            SupportActionBar.SetDisplayShowTitleEnabled(true); //show the title in the action bar

            var imm = (InputMethodManager)GetSystemService(Context.InputMethodService);

            //hides the keyboard
            imm.HideSoftInputFromWindow(_Token, 0);

            edtSeach.ClearFocus();

            //add the search icon in the action bar
            _SearchAction.SetIcon(ContextCompat.GetDrawable(this, Resource.Drawable.Search48));

            isSearchOpened = false;
        }

        public override void OnBackPressed()
        {
            if(isSearchOpened)
            {
                DisableSearch();
                return;
            }

            base.OnBackPressed();
        }

        private void DeleteItem()
        {
            RunOnUiThread(() =>
            {
                AppStartup.Current.Manager.UserRepository.Delete(_selectedId);

                var users = AppStartup.Current.Manager.UserRepository.Get().ToList();

                FillListView(users);

                FindViewById<FloatingActionButton>(Resource.Id.fab).Visibility = ViewStates.Gone;

                Toast.MakeText(this, "Item deleted.", ToastLength.Short).Show();
            });
        }

        private void OnInsertClick()
        {
            var name = FindViewById<EditText>(Resource.Id.editTextName);
            var email = FindViewById<EditText>(Resource.Id.editTextEmail);

            AppStartup.Current.Manager.UserRepository.Save(new Core.Data.Entity.User()
            {
                Name = name.Text,
                Email = email.Text
            });

            var users = AppStartup.Current.Manager.UserRepository.Get();

            FillListView(users.ToList());

            Toast.MakeText(this, "User inserted.", ToastLength.Short).Show();
        }

        private void FillListView(List<Core.Data.Entity.User> users)
        {
            RunOnUiThread(() =>
            {
                try
                {
                    ListView list = FindViewById<ListView>(Resource.Id.myListView);

                    var userList = new List<System.String>();
                    users.ForEach(user => userList.Add(string.Format(user.Id + "-" + user.Name + " - " + user.Email)));
                    list.Adapter = new ArrayAdapter<System.String>(this, Resource.Layout.ListItem, Resource.Id.textViewListItem, userList.ToArray());

                    list.OnItemClickListener = this;
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }
            });
        }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);

            string field = parent.GetItemAtPosition(position).ToString();

            _selectedId = Convert.ToInt32(field.Split('-').FirstOrDefault());

            fab.Visibility = Android.Views.ViewStates.Visible;

        }

        public bool OnEditorAction(TextView v, [GeneratedEnum] ImeAction actionId, KeyEvent e)
        {
            if (actionId == ImeAction.Search)
            {
                DoSearch(v.Text);
                return true;
            }
            return false;
        }

        private void DoSearch(string aSearch)
        {
            var user = new List<Core.Data.Entity.User>();
            
            //Searching users
            var users = AppStartup.Current.Manager.UserRepository.GetByFilters(a=>a.Name.Contains(aSearch)).ToList();

            FillListView(users);

            DisableSearch();
        }
    }
}

