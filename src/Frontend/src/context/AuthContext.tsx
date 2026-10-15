import React, { createContext, useContext, useState, useEffect } from 'react';
import { UserProfile, UserStage, UserObjective } from '../types';
import { api } from '../services/api';

interface AuthContextType {
  user: UserProfile | null;
  token: string | null;
  isLoading: boolean;
  login: (email: string, pass: string) => Promise<boolean>;
  loginDemo: () => Promise<boolean>;
  register: (data: any) => Promise<boolean>;
  logout: () => void;
  refreshUser: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [token, setToken] = useState<string | null>(localStorage.getItem('bj_token'));
  const [user, setUser] = useState<UserProfile | null>(null);
  const [isLoading, setIsLoading] = useState<boolean>(true);

  const fetchCurrentUser = async () => {
    try {
      if (!token) {
        setUser(null);
        setIsLoading(false);
        return;
      }
      const res = await api.getMe();
      if (res.success && res.data) {
        setUser(res.data);
      } else {
        logout();
      }
    } catch {
      // In local dev without backend running, fallback demo user
      if (token) {
        setUser({
          id: '99999999-9999-9999-9999-999999999999',
          email: 'demo@builderjourney.io',
          fullName: 'Alex Carter',
          role: 'Admin',
          currentStage: 'Prototype',
          primaryObjective: 'BuildProduct',
          preferredPlatform: 'X',
          hoursAvailablePerWeek: 15,
          skillsAndConstraints: 'TypeScript, React, C#; building evenings & weekends.',
          createdAt: new Date().toISOString(),
        });
      }
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchCurrentUser();
  }, [token]);

  const login = async (email: string, pass: string): Promise<boolean> => {
    try {
      const res = await api.login(email, pass);
      if (res.success && res.data?.token) {
        localStorage.setItem('bj_token', res.data.token);
        setToken(res.data.token);
        return true;
      }
      return false;
    } catch {
      return false;
    }
  };

  const loginDemo = async (): Promise<boolean> => {
    const success = await login('demo@builderjourney.io', 'Password123!');
    if (!success) {
      // Local fallback token
      const mockToken = 'mock_jwt_demo_token';
      localStorage.setItem('bj_token', mockToken);
      setToken(mockToken);
      setUser({
        id: '99999999-9999-9999-9999-999999999999',
        email: 'demo@builderjourney.io',
        fullName: 'Alex Carter',
        role: 'Admin',
        currentStage: 'Prototype',
        primaryObjective: 'BuildProduct',
        preferredPlatform: 'X',
        hoursAvailablePerWeek: 15,
        skillsAndConstraints: 'TypeScript, React, C#; building evenings & weekends.',
        createdAt: new Date().toISOString(),
      });
      return true;
    }
    return true;
  };

  const register = async (data: any): Promise<boolean> => {
    try {
      const res = await api.register(data);
      if (res.success && res.data?.token) {
        localStorage.setItem('bj_token', res.data.token);
        setToken(res.data.token);
        return true;
      }
      return false;
    } catch {
      return false;
    }
  };

  const logout = () => {
    localStorage.removeItem('bj_token');
    setToken(null);
    setUser(null);
  };

  const refreshUser = async () => {
    await fetchCurrentUser();
  };

  return (
    <AuthContext.Provider value={{ user, token, isLoading, login, loginDemo, register, logout, refreshUser }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) throw new Error('useAuth must be used within an AuthProvider');
  return context;
};
