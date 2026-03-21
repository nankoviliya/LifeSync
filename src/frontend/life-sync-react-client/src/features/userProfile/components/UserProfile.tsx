import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { UserProfileDataContainer } from '@/features/userProfile/components/profileData/UserProfileDataContainer';
import { useAuth } from '@/stores/AuthProvider';

export const UserProfile = () => {
  const { isLoading, isAuthenticated, user } = useAuth();

  return (
    <div className="p-4">
      {isLoading && <p>Loading...</p>}
      {!isLoading && !user && <p>Unable to find user profile data</p>}
      {isAuthenticated && user && (
        <div className="flex flex-col items-center gap-6 md:flex-row md:items-start">
          <Avatar className="h-24 w-24">
            <AvatarImage src="/default-user-avatar.png" alt="user_avatar" />
            <AvatarFallback>
              {user.email?.substring(0, 2).toUpperCase() ?? 'U'}
            </AvatarFallback>
          </Avatar>
          <UserProfileDataContainer userData={user} />
        </div>
      )}
    </div>
  );
};
