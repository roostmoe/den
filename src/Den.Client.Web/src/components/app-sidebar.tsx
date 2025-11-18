import { Sidebar, SidebarContent, SidebarFooter, SidebarGroup, SidebarGroupContent, SidebarGroupLabel, SidebarHeader, SidebarMenu, SidebarMenuBadge, SidebarMenuButton, SidebarMenuItem, SidebarMenuSub, SidebarMenuSubItem } from "@/components/ui/sidebar";
import { useMeQuery } from "@/lib/state/queries/auth";
import { Collapsible, CollapsibleContent, CollapsibleTrigger } from "@/components/ui/collapsible";
import { Link } from "@tanstack/react-router";
import { t } from "i18next";
import { CalendarIcon, CirclePoundSterlingIcon, Home, HomeIcon, LightbulbIcon, ShoppingBasketIcon, User2Icon, UtensilsIcon, type LucideIcon } from "lucide-react";
import { DropdownMenu, DropdownMenuTrigger } from "@/components/ui/dropdown-menu";
import { Skeleton } from "@/components/ui/skeleton";

type MenuItem = {
  type: 'group';
  title: string;
  children: MenuItem[];
} | {
  type: 'submenu';
  title: string;
  icon?: LucideIcon;
  defaultOpen?: boolean;
  children: MenuItem[];
} | {
  type: 'link';
  title: string;
  url: string;
  badge?: string | number;
  icon?: LucideIcon;
};

const items = [
  {
    type: 'group',
    title: t('menu.home.title'),
    children: [
      {
        type: 'link',
        title: t('menu.home.dashboard'),
        url: '#',
        icon: Home
      }
    ],
  },

  {
    type: 'group',
    title: t('menu.organisation.title'),
    children: [
      {
        type: 'link',
        title: t('menu.organisation.groceries'),
        url: '#',
        icon: ShoppingBasketIcon,
        badge: 15,
      },
      {
        type: 'link',
        title: t('menu.organisation.calendar'),
        url: '#',
        icon: CalendarIcon,
        badge: 3,
      },
      {
        type: 'link',
        title: t('menu.organisation.reminders'),
        url: '#',
        icon: LightbulbIcon,
        badge: 10,
      },
      {
        type: 'link',
        title: t('menu.organisation.recipes'),
        url: '#',
        icon: UtensilsIcon,
        badge: 56
      },
    ],
  },

  {
    type: 'group',
    title: t('menu.budgeting.title'),
    children: [
      {
        type: 'submenu',
        title: t('menu.budgeting.budgets'),
        icon: CirclePoundSterlingIcon,
        children: [
          {
            type: 'link',
            title: 'Household',
            url: '#',
          },
          {
            type: 'link',
            title: 'Grocery',
            url: '#',
          },
          {
            type: 'link',
            title: 'Bills & Services',
            url: '#',
          },
          {
            type: 'link',
            title: 'Disposable Income',
            url: '#',
          },
          {
            type: 'link',
            title: t('menu.budgeting.allBudgets'),
            url: '#',
          },
        ]
      },
    ],
  }
] as MenuItem[];

export const AppSidebar = () => {
  const { data, isLoading } = useMeQuery();

  return (
    <Sidebar>
      <SidebarHeader>
        <SidebarMenu>
          <SidebarMenuItem>
            <SidebarMenuButton size="lg">
              <div className="flex items-center gap-3">
                <div className="size-8 bg-primary flex items-center justify-center rounded-md">
                  <HomeIcon />
                </div>
                <span>Den</span>
              </div>
            </SidebarMenuButton>
          </SidebarMenuItem>
        </SidebarMenu>
      </SidebarHeader>

      <SidebarContent>
        <SidebarIterator itemList={items} />
      </SidebarContent>
      <SidebarFooter>
        <SidebarMenu>
          <SidebarMenuItem>
            {isLoading && !data
              ? (<Skeleton />)
              : (
                <DropdownMenu>
                  <DropdownMenuTrigger asChild>
                    <SidebarMenuItem>
                      <User2Icon /> {data?.username}
                    </SidebarMenuItem>
                  </DropdownMenuTrigger>
                </DropdownMenu>
              )
            }
          </SidebarMenuItem>
        </SidebarMenu>
      </SidebarFooter>
    </Sidebar>
  );
};

export const SidebarIterator = ({ parent, itemList }: { parent?: MenuItem, itemList: typeof items }) => {
  return itemList.map((item) => {
    switch (item.type) {
      case 'group':
        return (
          <SidebarGroup key={item.title}>
            <SidebarGroupLabel>{item.title}</SidebarGroupLabel>
            <SidebarGroupContent>
              <SidebarMenu>
                <SidebarIterator parent={item} itemList={item.children} />
              </SidebarMenu>
            </SidebarGroupContent>
          </SidebarGroup>
        );

      case 'submenu':
        return (
          <Collapsible key={item.title} defaultOpen={item.defaultOpen == null ? false : item.defaultOpen}>
            <SidebarMenuItem>
              <CollapsibleTrigger asChild>
                <SidebarMenuButton>
                  {item.icon && <item.icon />}
                  <span>{item.title}</span>
                </SidebarMenuButton>
              </CollapsibleTrigger>
              <CollapsibleContent>
                <SidebarMenuSub>
                  <SidebarIterator parent={item} itemList={item.children} />
                </SidebarMenuSub>
              </CollapsibleContent>
            </SidebarMenuItem>
          </Collapsible>
        );

      case 'link':
        let MenuItemComponent = SidebarMenuItem;
        if (parent && parent.type == 'submenu') {
          MenuItemComponent = SidebarMenuSubItem;
        }

        return (
          <MenuItemComponent key={item.title}>
            <SidebarMenuButton asChild>
              <Link {...item.url.startsWith('http') ? { href: item.url } : { to: item.url }}>
                {item.icon && <item.icon />}
                <span>{item.title}</span>
              </Link>
            </SidebarMenuButton>
            {item.badge && <SidebarMenuBadge>{item.badge}</SidebarMenuBadge>}
          </MenuItemComponent>
        );

      default:
        throw new Error('Invalid list item type.');
    }
  });
};
