import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';
import { Movie, MovieService } from '../../services/movie';

@Component({
  selector: 'app-movies',
  imports: [RouterLink, FormsModule],
  templateUrl: './movies.html',
  styleUrl: './movies.css'
})
export class Movies implements OnInit {
  searchText = '';
  selectedGenre = 'All';
  genres = ['All'];
  movies: Movie[] = [];
  totalCount = 0;
  isLoading = false;
  errorMessage = '';

  constructor(private movieService: MovieService) {}

  ngOnInit(): void {
    this.loadMovies();
  }

  loadMovies(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.movieService.getMovies(this.searchText, this.selectedGenre)
      .pipe(
        finalize(() => {
          this.isLoading = false;
        })
      )
      .subscribe({
        next: response => {
          this.movies = response.items ?? [];
          this.totalCount = response.totalCount ?? 0;

          this.genres = [
            'All',
            ...new Set(
              this.movies
                .map(movie => movie.genre)
                .filter(Boolean)
            )
          ];
        },
        error: error => {
          this.errorMessage = 'Unable to load movies. Please check the backend.';
          console.error('Movies API error:', error);
        }
      });
  }

  searchMovies(): void {
    this.loadMovies();
  }

  selectGenre(genre: string): void {
    this.selectedGenre = genre;
    this.loadMovies();
  }

  getDuration(minutes: number): string {
    const hours = Math.floor(minutes / 60);
    const remainingMinutes = minutes % 60;
    return `${hours}h ${remainingMinutes}m`;
  }
}
