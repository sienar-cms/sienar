import { provide } from '@sienar/react-utils';
import { DASHBOARD_VIEW, DASHBOARD_LAYOUT } from '@sienar/react-client-mui';
import Dashboard from './Dashboard.tsx';
import DashboardLayout from './DashboardLayout.tsx';

provide(DASHBOARD_LAYOUT, <DashboardLayout/>);
provide(DASHBOARD_VIEW, <Dashboard/>);