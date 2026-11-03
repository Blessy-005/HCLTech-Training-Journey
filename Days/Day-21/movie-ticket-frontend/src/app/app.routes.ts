import { Routes } from '@angular/router';
import { Home } from './pages/home/home';
import { Movies } from './pages/movies/movies';
import { MovieDetails } from './pages/movie-details/movie-details';
import { SeatSelection } from './pages/seat-selection/seat-selection';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';

export const routes: Routes = [
  { path: '', component: Home },
  { path: 'home', component: Home },
  { path: 'movies', component: Movies },
  { path: 'movie-details/:id', component: MovieDetails },
  { path: 'seats', component: SeatSelection },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: '**', redirectTo: '' }
];
