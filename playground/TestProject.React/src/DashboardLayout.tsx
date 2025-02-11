import { Outlet } from 'react-router-dom';
import 'bulma/css/bulma.css';

export default function Dashboard() {
	return (
		<div>
			<Outlet/>
		</div>
	);
}