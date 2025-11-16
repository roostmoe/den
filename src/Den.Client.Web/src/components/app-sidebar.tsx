import { Sidebar, SidebarContent, SidebarFooter, SidebarGroup, SidebarGroupContent, SidebarGroupLabel, SidebarHeader, SidebarMenu, SidebarMenuButton, SidebarMenuItem } from "@/components/ui/sidebar";
import { Link } from "@tanstack/react-router";
import { Home, HomeIcon, type LucideIcon } from "lucide-react";

type MenuItem = {
  type: 'group';
  title: string;
  children: MenuItem[];
} | {
  type: 'link';
  title: string;
  url: string;
  icon: LucideIcon;
};

const items = [
  {
    type: 'group',
    title: 'Application',
    children: [
      {
        type: 'link',
        title: 'Home',
        url: '#',
        icon: Home
      },
      {
        type: 'link',
        title: 'GitHub',
        url: 'https://github.com/roostmoe/den',
        icon: Home
      }
    ],
  }
] as MenuItem[];

export const AppSidebar = () => {
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
      <SidebarFooter />
    </Sidebar>
  );
};

export const SidebarIterator = ({ itemList }: { itemList: typeof items }) => {
  return itemList.map((item) => {
    switch (item.type) {
      case 'group':
        return (
          <SidebarGroup>
            <SidebarGroupLabel>{item.title}</SidebarGroupLabel>
            <SidebarGroupContent>
              <SidebarMenu>
                <SidebarIterator itemList={item.children} />
              </SidebarMenu>
            </SidebarGroupContent>
          </SidebarGroup>
        );

      case 'link':
        return (
          <SidebarMenuItem key={item.title}>
            <SidebarMenuButton asChild>
              <Link {...item.url.startsWith('http') ? { href: item.url } : { to: item.url }}>
                <item.icon />
                <span>{item.title}</span>
              </Link>
            </SidebarMenuButton>
          </SidebarMenuItem>
        );

      default:
        throw new Error('Invalid list item type.');
    }
  });
};
