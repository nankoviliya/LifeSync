import { useEffect } from 'react';
import { useLocation } from 'react-router-dom';

export const SessionExpiredNotification = () => {
  const location = useLocation();

  useEffect(() => {
    const searchParams = new URLSearchParams(location.search);
    const sessionExpired = searchParams.get('session_expired');

    if (sessionExpired === 'true') {
      // TODO: Show toast notification when toast system is available
      // toast.error('Your session has expired. Please log in again.');
      console.warn('Session expired. Please log in again.');

      // Clean URL after showing message
      searchParams.delete('session_expired');
      const newSearch = searchParams.toString();
      const newUrl = `${location.pathname}${newSearch ? '?' + newSearch : ''}`;
      window.history.replaceState({}, '', newUrl);
    }
  }, [location]);

  return null;
};
