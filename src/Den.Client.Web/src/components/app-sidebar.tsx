import { Sidebar, SidebarContent, SidebarFooter, SidebarGroup, SidebarGroupContent, SidebarGroupLabel, SidebarHeader, SidebarMenu, SidebarMenuBadge, SidebarMenuButton, SidebarMenuItem, SidebarMenuSub, SidebarMenuSubItem } from "@/components/ui/sidebar";
import { Collapsible, CollapsibleContent, CollapsibleTrigger } from "@/components/ui/collapsible";
import { CalendarIcon, ChevronUpIcon, CirclePoundSterlingIcon, CogIcon, Home, HomeIcon, LightbulbIcon, ShoppingBasketIcon, User2Icon, UsersIcon, UtensilsIcon, type LucideIcon } from "lucide-react";
import { DropdownMenu, DropdownMenuItem, DropdownMenuTrigger,  DropdownMenuContent, DropdownMenuLabel, DropdownMenuSeparator } from "@/components/ui/dropdown-menu";
import { Skeleton } from "@/components/ui/skeleton";
import { useMeQuery } from "@/lib/state/queries/auth";
import { t } from "i18next";
import { Link } from "@tanstack/react-router";
import { useAuth } from "@/lib/state/auth";
import { useEffect, useState } from "react";
import { useBudgetsListQuery } from "@/lib/state/queries/budgets";

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

export const AppSidebar = () => {
  const [items, setItems] = useState<MenuItem[]>([
    {
      type: 'group',
      title: t('menu.home.title'),
      children: [
        {
          type: 'link',
          title: t('menu.home.dashboard'),
          url: '/',
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
              title: t('menu.budgeting.allBudgets'),
              url: '/budgets',
            },
          ]
        },
      ],
    },

    {
      type: 'group',
      title: t('menu.admin.title'),
      children: [
        {
          type: 'link',
          title: t('menu.admin.settings'),
          url: '#',
          icon: CogIcon,
        },
        {
          type: 'link',
          title: t('menu.admin.users'),
          url: '#',
          icon: UsersIcon,
        },
      ],
    },
  ]);

  const { logout } = useAuth();
  const { data, isLoading } = useMeQuery();
  const { data: budgetsData } = useBudgetsListQuery();

  useEffect(() => {
    if (budgetsData) {
      setItems((prevItems) => {
        const budgetingGroupIndex = prevItems.findIndex(
          (item) => item.type === 'group' && item.title === t('menu.budgeting.title')
        );

        if (budgetingGroupIndex === -1) return prevItems;

        const budgetingGroup = prevItems[budgetingGroupIndex];
        if (budgetingGroup.type !== 'group') return prevItems;

        const budgetsSubmenuIndex = budgetingGroup.children.findIndex(
          (child) => child.type === 'submenu' && child.title === t('menu.budgeting.budgets')
        );

        if (budgetsSubmenuIndex === -1) return prevItems;

        const budgetsSubmenu = budgetingGroup.children[budgetsSubmenuIndex];
        if (budgetsSubmenu.type !== 'submenu') return prevItems;

        const dynamicBudgetItems: MenuItem[] = budgetsData.map((budget) => ({
          type: 'link',
          title: budget.displayName,
          url: `/budgets/${budget.id}`,
        }));

        const updatedBudgetsSubmenu = {
          ...budgetsSubmenu,
          children: [
            {
              type: 'link' as const,
              title: t('menu.budgeting.allBudgets'),
              url: '/budgets',
            },
            ...dynamicBudgetItems,
          ],
        };

        const updatedBudgetingGroup = {
          ...budgetingGroup,
          children: [
            ...budgetingGroup.children.slice(0, budgetsSubmenuIndex),
            updatedBudgetsSubmenu,
            ...budgetingGroup.children.slice(budgetsSubmenuIndex + 1),
          ],
        };

        return [
          ...prevItems.slice(0, budgetingGroupIndex),
          updatedBudgetingGroup,
          ...prevItems.slice(budgetingGroupIndex + 1),
        ];
      });
    }
  }, [budgetsData]);

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
                    <SidebarMenuButton>
                      <User2Icon /> {data?.username}
                      <ChevronUpIcon className="ml-auto" />
                    </SidebarMenuButton>
                  </DropdownMenuTrigger>
                  <DropdownMenuContent side="top" className="w-[--radix-popper-anchor-width]">
                    <DropdownMenuLabel>Account</DropdownMenuLabel>
                    <DropdownMenuItem>
                      <span>Profile</span>
                    </DropdownMenuItem>
                    <DropdownMenuItem>
                      <span>Settings</span>
                    </DropdownMenuItem>
                    <DropdownMenuSeparator />
                    <DropdownMenuItem
                      className="cursor-pointer"
                      onClick={e => { e.preventDefault(); logout() }}
                    >
                      <span>Log out</span>
                    </DropdownMenuItem>
                  </DropdownMenuContent>
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
