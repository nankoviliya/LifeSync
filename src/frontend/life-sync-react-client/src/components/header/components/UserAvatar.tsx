import { Button } from 'primereact/button';
import { Menu } from 'primereact/menu';

import { useUserAvatar } from '@/components/header/hooks/useUserAvatar';

export const UserAvatar = () => {
  const { avatarMenuRef, avatarMenuItems, toggleAvatarMenu } = useUserAvatar();

  return (
    <div>
      <Button
        icon="pi pi-user"
        onClick={toggleAvatarMenu}
        rounded
        text
        aria-label="User avatar"
        aria-haspopup="menu"
        aria-controls="avatar-menu"
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
