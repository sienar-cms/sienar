import { provide } from '@sienar/react-utils';
import { DASHBOARD_VIEW } from '@sienar/react-client-mui';
import Dashboard from './Dashboard.tsx';

provide(DASHBOARD_VIEW, <Dashboard/>);