import { HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { HttpService } from '../services/http/http.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private _httpService: HttpService, private router: Router) { }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): any {

    const isAccessTokenValid = this._httpService.get('api/jwt/validate/access', new HttpHeaders({ 'Content-Type': 'application/json' }), true);

    return isAccessTokenValid.then((data) => {
      if (!data)
        return this._httpService.post('api/jwt/refresh', {}, new HttpHeaders({ 'Content-Type': 'application/json' }), true).then((data) => { return true; })
          .catch((err) => {
            this.router.navigate(['user/signin']);
            return false;
          });
      else
        return true;
    });
  }
}
