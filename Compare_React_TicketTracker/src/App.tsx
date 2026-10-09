import { Link, Navigate, Route, Routes } from 'react-router-dom';
import { RequireAuth, useAuth } from './auth';
import LoginPage from './LoginPage';
import TicketDetailPage from './TicketDetailPage';
import TicketsPage from './TicketsPage';

export default function App() {
  const { username, logout } = useAuth();

  return (
    <div className="app">
      <header>
        <h1>
          <Link to="/">Ticket Tracker</Link>
        </h1>
        <div className="header-right">
          {username && (
            <>
              <span className="user">{username}</span>
              <button onClick={logout}>Abmelden</button>
            </>
          )}
          <span className="badge-tech react">React</span>
        </div>
      </header>

      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/" element={<RequireAuth><TicketsPage /></RequireAuth>} />
        <Route path="/tickets/:id" element={<RequireAuth><TicketDetailPage /></RequireAuth>} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </div>
  );
}
