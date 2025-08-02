using UnityEngine;

public class MenuManager : MonoBehaviour
{
        public Menu[] menus;
        public static MenuManager Instance;
        
        private void OnEnable()
        {
            //EventManager.OnHomeButtonClicked += OpenHomeMenu;
           // EventManager.OnObjectClicked += OpenRedTowersMenu;
        }
    
        private void OnDisable()
        {
           // EventManager.OnHomeButtonClicked -= OpenHomeMenu;
           // EventManager.OnObjectClicked -= OpenRedTowersMenu;
        }
    
        void Awake()
        {
            Instance = this;
        }
    
        public void OpenMenu(MenuType menuToOpen)
        {
            foreach (var menu in menus)
            {
                if (menu.menuType == menuToOpen)
                {
                    menu.Open();
                }
                else
                {
                    menu.Close();
                }
            }
        }
    
        public void OpenMenu(Menu menuToOpen)
        {
            foreach (var menu in menus)
            {
                menu.Close();
            }
            menuToOpen.Open();
        }
    
        public void CloseMenu(Menu menu)
        {
            menu.Close();
        }
        
        
        public void OpenHomeMenu()
        {
            OpenMenu(MenuType.HomeMenu);
        }
        
        public void OpenRedTowersMenu(Transform focusPoint)
        {
            OpenMenu(MenuType.RedTowersMenu);
        }
}
