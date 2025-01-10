import styles from './Footer.module.scss';

export const Footer = () => {
  const currentYear = new Date().getFullYear();

  return (
    <footer className={styles['footer']}>
      <div className={styles['footer__content']}>
        <p className={styles['footer__text']}>
          &copy; {currentYear} nankoviliya. All Rights Reserved.
        </p>
        <div className={styles['footer__links']}>
          <a
            href="https://google.com"
            target="_blank"
            rel="noopener noreferrer"
            className={styles['footer__link']}
          >
            Portfolio
            <i className="pi pi-external-link"></i>
          </a>
          <a
            href="https://github.com/nankoviliya"
            target="_blank"
            rel="noopener noreferrer"
            className={styles['footer__link']}
          >
            GitHub
            <i className="pi pi-github"></i>
          </a>
          <a
            href="https://www.linkedin.com/in/iliya-nankov/"
            target="_blank"
            rel="noopener noreferrer"
            className={styles['footer__link']}
          >
            LinkedIn
            <i className="pi pi-linkedin"></i>
          </a>
        </div>
      </div>
    </footer>
  );
};
