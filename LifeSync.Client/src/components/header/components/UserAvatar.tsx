import { useUserAvatar } from '@/components/header/hooks/useUserAvatar';
import { Avatar } from 'primereact/avatar';
import { Menu } from 'primereact/menu';

export const UserAvatar = () => {
  const { avatarMenuRef, avatarMenuItems, toggleAvatarMenu } = useUserAvatar();

  return (
    <div>
      <Avatar
        className=""
        icon="pi pi-user"
        size="normal"
        shape="circle"
        onClick={toggleAvatarMenu}
      />
      <Menu
        model={avatarMenuItems}
        popup
        ref={avatarMenuRef}
        id="avatar-menu"
        popupAlignment="left"
      />
    </div>
  );
};
