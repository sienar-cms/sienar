import { Button } from '@sienar/react-ui-mui';

export default function Dashboard() {
	return (
		<div>
			<Button
				href='/dashboard/account/login'
			>
				Click me!!
			</Button>
			<Button
				className='just-another-class'
			>
				Not me, you idiot, HIM!
			</Button>
			<Button
				to='/dashboard/account/login'
			>
				Login in with React Router
			</Button>
		</div>
	);
}