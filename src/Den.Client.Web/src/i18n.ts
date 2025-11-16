import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';

export const defaultNS = "main";
export const resources = {
  en: {
    main: {
      // App Strings
      appTitle: 'Den',

      // Dashboard
      dashboard: {
        title: 'Dashboard',
      },

      // Login Form
      login: {
        page: { title: 'Log in' },
        form: {
          title: 'Log in to your account',
          subtitle: 'Enter your username and password below.',

          usernameLabel: 'Username',
          passwordLabel: 'Password',
          forgotPassword: 'Forgot your password?',
          rememberLabel: 'Remember Me?',
          doLogin: 'Log in',
        },
      },

      // Signup Form
      signup: {
        page: { title: 'Sign up' },
        form: {
          title: 'Create an account',
          subtitle: 'Sign up for this Den instance.',
          usernameLabel: 'Username',
          usernameDescription: 'This is the username you use to log in to Den.',
          emailLabel: 'Email address',
          emailDescription: 'We never share your email with anyone.',
          passwordLabel: 'Password',
          passwordConfirmLabel: 'Confirm Password',
          doSignup: 'Sign up',
        },
      },

      // Registration Form
    },
  },
};

i18n
  .use(initReactI18next)
  .init({
    lng: 'en',
    interpolation: {
      escapeValue: false,
    },
    ns: ["main"],
    defaultNS,
    resources,
  });

export default i18n;
